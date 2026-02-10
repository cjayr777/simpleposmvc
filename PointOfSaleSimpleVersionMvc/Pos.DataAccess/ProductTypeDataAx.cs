using Npgsql;
using Pos.Model;
using Pos.Tabular;
using Proj.Util;
using System.Data;

namespace Pos.DataAccess;

public class ProductTypeDataAx
{
    public ProductTypeDataAx(PostgreHelper p_pgh)
    {
        pgh = p_pgh;
    }


    PostgreHelper pgh;


    public async Task<OpResult> AddAsync(ProductType mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.Name, mod.Name),
                (ParamName.Details, mod.Details),
                (ParamName.SortOrder, mod.SortOrder)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("producttype_insert", args);

        return result;
    }

    public async Task<OpResult> DeleteAsync(ProductType mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.ProductTypeId, mod.ProductTypeId),
                (ParamName.Name, mod.Name)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("producttype_delete", args);

        return result;
    }

    public async Task<OpResult> EditAsync(ProductType mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.ProductTypeId, mod.ProductTypeId),
                (ParamName.Name, mod.Name),
                (ParamName.Details, mod.Details),
                (ParamName.SortOrder, mod.SortOrder)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("producttype_update", args);

        return result;
    }

    public async Task<DataResult<DataTable>> GetAsync(ProductType mod, string search = "")
    {
        var args = pgh.Args
            (
                (ParamName.ProductTypeId, mod.ProductTypeId),
                (ParamName.Search, search)
            );

        var dr = new DataResult<DataTable>();

        try
        {
            DataTable dt = await pgh.CallFunctionQueryAsync("producttype_acquire", args);

            return DataResult<DataTable>.Ok(dt);
        }
        catch (PostgresException pgEx)
        {
            dr = DataResult<DataTable>.Fail(new DataTable(), $"Unexpected PostgreSQL error occurred.\n{pgEx.MessageText}");

            return dr;
        }
        catch (Exception ex)
        {
            dr = DataResult<DataTable>.Fail(new DataTable(), $"Unexpected error occurred.\n{ex.Message}");

            return dr;
        }
    }

}
