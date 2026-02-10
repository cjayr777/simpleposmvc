using Npgsql;
using System.Data;

namespace Proj.Util;

// VERSION 2.1
public sealed class PostgreHelper
{
    public PostgreHelper(string p_conString)
    {
        con = p_conString;
    }


    string con;


    #region Connection And Command

    private NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(con);
    }

    private static void AddArgs(NpgsqlCommand cmd, Dictionary<string, object?>? args)
    {
        if (args == null || args.Count == 0) return;

        foreach (var p in args) 
        {
            cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
        }
    }

    private static string BuildArgsList(Dictionary<string, object?>? args)
    {
        if (args == null || args.Count == 0) return "";

        string result = string.Join(", ", args.Keys.Select(k => "@" + k));

        return result;
    }

    #endregion


    #region Function - Scalars

    public async Task<T?> CallFunctionScalarAsync<T>(string storedFunctionName, Dictionary<string, object?>? args)
    {
        string sql = $"SELECT {storedFunctionName} ({BuildArgsList(args)});";

        return await ExecuteScalarAsync<T>(sql, args);
    }

    private async Task<T?> ExecuteScalarAsync<T>(string sql, Dictionary<string, object?>? args)
    {
        await using var conn = CreateConnection();
        await using var cmd = new NpgsqlCommand(sql, conn);

        AddArgs(cmd, args);

        await conn.OpenAsync();

        object? result = await cmd.ExecuteScalarAsync();

        if (result == null || result is DBNull) return default;

        return (T) Convert.ChangeType(result, typeof(T));
    }

    #endregion


    #region Function - Command (TEXT result -> OpResult)

    /// <summary>
    /// Calls a PostgreSQL function that returns TEXT
    /// </summary>
    /// <param name="storedFunctionName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public async Task<OpResult> CallFunctionOpResultAsync(string storedFunctionName, Dictionary<string, object?>? args)
    {
        try
        {
            string? result = await CallFunctionScalarAsync<string>(storedFunctionName, args);

            return string.IsNullOrWhiteSpace(result)
                ? OpResult.Ok()
                : OpResult.Fail(result);
        }
        catch (Exception ex)
        {
            return OpResult.Fail(ex.Message);
        }
    }

    #endregion


    #region Function - Query (DataTable)

    /// <summary>
    /// Calls a PostgreSQL function returning a result set
    /// Used for joins / extended projections
    /// </summary>
    /* sample use

    DROP FUNCTION IF EXISTS grade_select;
    CREATE OR REPLACE FUNCTION grade_select(
        p_grade_id INT DEFAULT 0,
        p_search VARCHAR DEFAULT ''
    )
    RETURNS TABLE(
        grade_id INT,
        name VARCHAR,
        details VARCHAR,
        sort_order INT,
        stat_id INT,
        stat_name VARCHAR,
        error_message TEXT  -- Add error column
    )
    LANGUAGE plpgsql
    AS $$
    DECLARE
        v_error_message TEXT;
    BEGIN
        -- Initialize
        v_error_message := '';
    p_search := TRIM(COALESCE(p_search, ''));

        -- Handle the query with exception block
        BEGIN
            RETURN QUERY
            SELECT
                g.grade_id,
                g.name,
                g.details,
                g.sort_order,
                g.stat_id,
                s.name as stat_name,
                ''::TEXT as error_message  -- Empty error for successful rows
            FROM grades g
            LEFT JOIN stats s ON g.stat_id = s.stat_id
            WHERE g.stat_id != 0
              AND(p_grade_id = 0 OR g.grade_id = p_grade_id)
              AND(
                    g.name ILIKE '%' || p_search || '%'
                OR g.details ILIKE '%' || p_search || '%'
                OR p_search = ''
              );
    EXCEPTION
        WHEN OTHERS THEN
                -- Return a single row with error details
                RETURN QUERY SELECT 
                    0, '', '', 0, 0, '',
                    'Error: ' || SQLERRM;
        END;
    END;
    $$;
     */
    public async Task<DataTable> CallFunctionQueryAsync(string storedFunctionName, Dictionary<string, object?>? args)
    {
        string sql = $"SELECT * FROM {storedFunctionName} ({BuildArgsList(args)});";

        return await ExecuteQueryAsync(sql, args);
    }

    public async Task<DataTable> CallFunctionQueryParamAsync(string storedFunctionName, Dictionary<string, object?>? args)
    {
        string sql = $"SELECT *FROM {storedFunctionName} ({BuildArgsList(args)}) WHERE (p_error_message = '' OR p_error_message IS NULL);;";

        return await ExecuteQueryAsync(sql, args);
    }

    private async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object?>? args)
    {
        await using var conn = CreateConnection();
        await using var cmd = new NpgsqlCommand(sql, conn);

        AddArgs(cmd, args);

        await conn.OpenAsync();

        await using var reader = await cmd.ExecuteReaderAsync();

        var table = new DataTable();
        table.Load(reader);

        return table;
    }

    #endregion

    #region Helpers

    public Dictionary<string, object?>? Args(params (string Name, object? Value)[] values)
    {
        return values.ToDictionary(v => v.Name, v => v.Value);
    }

    #endregion
}