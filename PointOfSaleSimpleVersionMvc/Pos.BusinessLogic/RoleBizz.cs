using Pos.DataAccess;
using Pos.Model;
using Pos.Tabular;
using Pos.ViewModel;
using Proj.Util;
using System.Data;

namespace Pos.BusinessLogic;

public class RoleBizz
{
    public RoleBizz(RoleDataAx p_db)
    {
        db = p_db;
    }


    private RoleDataAx db;


    private List<RoleVm> LocalSorter(SortMode sortMode, DataResult<DataTable> dtResult)
    {
        if (dtResult is null)
        {
            return new List<RoleVm>();
        }

        List<RoleVm> list = dtResult.Data.Rows
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

    private RoleVm RowToVm(DataRow p_dr)
    {
        RoleVm vm = new RoleVm();

        vm.RoleId = Turn.Int(p_dr[ColName.RoleId]);
        vm.Name = Turn.String(p_dr[ColName.Name]);
        vm.Details = Turn.String(p_dr[ColName.Details]);
        vm.SortOrder = Turn.Int(p_dr[ColName.SortOrder]);
        vm.StatId = Turn.Int(p_dr[ColName.StatId]);
        vm.StatName = Turn.String(p_dr[ColName.StatName]);

        return vm;
    }

    private Role VmToModel(RoleVm vm)
    {
        Role mod = new Role();

        mod.RoleId = vm.RoleId;
        mod.Name = vm.Name;
        mod.Details = vm.Details;
        mod.SortOrder = vm.SortOrder;
        mod.StatId = vm.StatId;

        return mod;
    }


    public async Task<OpResult> AddAsync(RoleVm vm)
    {
        var ec = new EntryCheck()
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name)
            .IsNotNegative(vm.SortOrder, ShowName.SortOrder);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        Role mod = VmToModel(vm);

        OpResult result = await db.AddAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<OpResult> DeleteAsync(RoleVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.RoleId, ShowName.Id)
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        Role mod = VmToModel(vm);

        OpResult result = await db.DeleteAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<OpResult> EditAsync(RoleVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.RoleId, ShowName.Id)
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name)
            .IsNotNegative(vm.SortOrder, ShowName.SortOrder);

        if (!ec.IsValid)
        {
            return OpResult.Fail(ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        Role mod = VmToModel(vm);

        OpResult result = await db.EditAsync(mod, vm.ActionerId);

        return result;
    }

    public async Task<DataResult<List<RoleVm>>> GetAsync(RoleVm vm, SortMode sortMode = SortMode.asc) 
    {
        Role mod = VmToModel(vm);
        var dtResult = await db.GetAsync(mod, vm.Search);

        if (!dtResult.IsSuccess || dtResult.Data is null)
        {
            return DataResult<List<RoleVm>>.Fail(new(), dtResult.Message);
        }

        List<RoleVm> list = LocalSorter(sortMode, dtResult);

        return DataResult<List<RoleVm>>.Ok(list);
    }

    public async Task<DataResult<RoleVm>> GetOneAsync(RoleVm vm)
    {
        var ec = new EntryCheck()
            .IsNotZeroOrLess(vm.RoleId, ShowName.Id)
            .IsNotNullOrWhitespace(vm.Name, ShowName.Name);

        if (!ec.IsValid)
        {
            return DataResult<RoleVm>.Fail(new(), ShowName.ValFailed + string.Join(", ", ec.Errors));
        }

        try
        {
            Role mod = VmToModel(vm);

            var dtResult = await db.GetAsync(mod, "");

            if (!dtResult.IsSuccess || dtResult.Data is null)
            {
                return DataResult<RoleVm>.Fail(new(), dtResult.Message);
            }

            RoleVm one = dtResult.Data.Rows
                .Cast<DataRow>()
                .Select(RowToVm)
                .SingleOrDefault()
                ?? new RoleVm();

            return DataResult<RoleVm>.Ok(one);
        }
        catch (Exception ex)
        {
            return DataResult<RoleVm>.Fail(new(), 
                $"{ShowName.UnexpectErr}{ex.Message}");
        }
    }
}
