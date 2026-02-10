using Pos.DataAccess;
using Pos.Model;
using Pos.Tabular;
using Pos.ViewModel;
using Proj.Util;
using System.Data;

namespace Pos.BusinessLogic;

public class ProductTypeBizz
{
    public ProductTypeBizz(ProductTypeDataAx p_db)
    {
        db = p_db;
    }


    ProductTypeDataAx db;


    private List<ProductTypeVm> LocalSorter(SortMode sortMode, DataResult<DataTable> dtResult)
    {
        if (dtResult.Data is null)
        {
            return new List<ProductTypeVm>();
        }

        List<ProductTypeVm> list = dtResult.Data.Rows
            .Cast<DataRow>()
            .Select(RowToVm)
            .ToList();

        if (list.Count > 1)
        {
            list = list
                .OrderByWithDirection(x => x.SortOrder, SortMode.desc)
                .ThenByWithDirection(x => x.Name, sortMode)
                .ToList();
        }

        return list;
    }

    private ProductTypeVm RowToVm(DataRow p_dr)
    {
        ProductTypeVm vm = new ProductTypeVm();

        vm.ProductTypeId = Turn.Int(p_dr[ColName.ProductTypeId]);
        vm.Name = Turn.String(p_dr[ColName.Name]);
        vm.Details = Turn.String(p_dr[ColName.Details]);
        vm.SortOrder = Turn.Int(p_dr[ColName.SortOrder]);

        return vm;
    }

    private ProductType VmToModel(ProductTypeVm vm)
    {
        ProductType mod = new ProductType();

        mod.ProductTypeId = vm.ProductTypeId;
        mod.Name = vm.Name;
        mod.Details = vm.Details;
        mod.SortOrder = vm.SortOrder;

        return mod;
    }


    public async Task<OpResult> AddAsync(ProductTypeVm vm)
    {
        var ec = new EntryCheck()
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name)
            .IsNotNegative(vm.SortOrder, ShowName.SortOrder);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        ProductType mod = VmToModel(vm);

        OpResult result = await db.AddAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<OpResult> DeleteAsync(ProductTypeVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.ProductTypeId, ShowName.Id)
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        ProductType mod = VmToModel(vm);

        OpResult result = await db.DeleteAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<OpResult> EditAsync(ProductTypeVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.ProductTypeId, ShowName.Id)
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name)
            .IsNotNegative(vm.SortOrder, ShowName.SortOrder);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        ProductType mod = VmToModel(vm);

        OpResult result = await db.EditAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<DataResult<List<ProductTypeVm>>> GetAsync(ProductTypeVm vm, SortMode sortMode = SortMode.asc)
    {
        ProductType mod = VmToModel(vm);

        var dtResult = await db.GetAsync(mod, vm.Search);

        if (!dtResult.IsSuccess || dtResult.Data is null)
        {
            return DataResult<List<ProductTypeVm>>.Fail(new(), dtResult.Message);
        }

        List<ProductTypeVm> list = LocalSorter(sortMode, dtResult);

        return DataResult<List<ProductTypeVm>>.Ok(list);
    }

    public async Task<DataResult<ProductTypeVm>> GetOneAsync(ProductTypeVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.ProductTypeId, ShowName.Id);

        if (!ec.IsValid)
        {
            return DataResult<ProductTypeVm>.Fail(new(), ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        try
        {
            ProductType mod = VmToModel(vm);

            var dtResult = await db.GetAsync(mod, "");

            if (!dtResult.IsSuccess || dtResult.Data is null)
            {
                return DataResult<ProductTypeVm>.Fail(new(), dtResult.Message);
            }

            ProductTypeVm one = dtResult.Data.Rows
                .Cast<DataRow>()
                .Select(RowToVm)
                .SingleOrDefault();

            return DataResult<ProductTypeVm>.Ok(one, "");
        }
        catch (Exception ex)
        {
            return DataResult<ProductTypeVm>.Fail(
                new(),
                $"{ShowName.UnexpectErr}{ex.Message}");
        }
    }

}
