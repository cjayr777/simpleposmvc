using Npgsql;
using Pos.Model;
using Pos.Tabular;
using Proj.Util;
using System.Data;

namespace Pos.DataAccess;

public class RoleDataAx
{
    public RoleDataAx(PostgreHelper p_pgh)
    {
        pgh = p_pgh;
    }


    PostgreHelper pgh;


    public async Task<OpResult> AddAsync(Role mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.Name, mod.Name),
                (ParamName.Details, mod.Details),
                (ParamName.SortOrder, mod.SortOrder)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("role_insert", args);

        return result;
    }

    public async Task<OpResult> DeleteAsync(Role mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.RoleId, mod.RoleId),
                (ParamName.Name, mod.Name)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("role_delete", args);

        return result;
    }

    public async Task<OpResult> EditAsync(Role mod, int actionerId = 0)
    {
        var args = pgh.Args
            (
                (ParamName.RoleId, mod.RoleId),
                (ParamName.Name, mod.Name),
                (ParamName.Details, mod.Details),
                (ParamName.SortOrder, mod.SortOrder)
            );

        OpResult result = await pgh.CallFunctionOpResultAsync("role_update", args);

        return result;
    }

    public async Task<DataResult<DataTable>> GetAsync(Role mod, string search = "")
    {
        var args = pgh.Args
            (
                (ParamName.RoleId, mod.RoleId),
                (ParamName.Search,search)
            );

        var dr = new DataResult<DataTable>();

        try
        {
            DataTable dt = await pgh.CallFunctionQueryAsync("role_acquire", args);

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
