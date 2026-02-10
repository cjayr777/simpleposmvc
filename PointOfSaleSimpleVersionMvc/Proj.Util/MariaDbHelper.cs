using MySql.Data.MySqlClient;
using System.Data;

namespace Proj.Util;

// VERSION 2.1 - MariaDB Edition
public sealed class MariaDbHelper
{
    public MariaDbHelper(string p_conString)
    {
        con = p_conString;
    }

    private string con;

    #region Connection And Command

    private MySqlConnection CreateConnection()
    {
        return new MySqlConnection(con);
    }

    private static void AddArgs(MySqlCommand cmd, Dictionary<string, object?>? args)
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

    #region Stored Procedure - Scalars (OUT parameters)

    /// <summary>
    /// Calls a MariaDB stored procedure with OUT parameter for scalar result
    /// </summary>
    public async Task<T?> CallProcedureScalarAsync<T>(string storedProcedureName, Dictionary<string, object?>? args)
    {
        await using var conn = CreateConnection();
        await using var cmd = new MySqlCommand(storedProcedureName, conn);
        cmd.CommandType = CommandType.StoredProcedure;

        AddArgs(cmd, args);

        // Add output parameter if not present
        if (!cmd.Parameters.Contains("@p_result"))
        {
            cmd.Parameters.Add("@p_result", MySqlDbType.VarChar, 4000).Direction = ParameterDirection.Output;
        }

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        var result = cmd.Parameters["@p_result"].Value;
        if (result == null || result is DBNull) return default;

        return (T)Convert.ChangeType(result, typeof(T));
    }

    #endregion

    #region Stored Procedure - Command (TEXT result -> OpResult)

    /// <summary>
    /// Calls a MariaDB stored procedure that returns result through OUT parameter
    /// </summary>
    public async Task<OpResult> CallProcedureOpResultAsync(string storedProcedureName, Dictionary<string, object?>? args)
    {
        try
        {
            string? result = await CallProcedureScalarAsync<string>(storedProcedureName, args);

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

    #region Stored Procedure - Query (DataTable) - MariaDB can return result sets directly

    /// <summary>
    /// Calls a MariaDB stored procedure returning a result set
    /// MariaDB stored procedures can return multiple result sets directly
    /// </summary>
    /* MariaDB Procedure Sample:
    
    DELIMITER $$
    CREATE PROCEDURE grade_select(
        IN p_grade_id INT,
        IN p_search VARCHAR(255),
        OUT p_error_message TEXT
    )
    BEGIN
        DECLARE EXIT HANDLER FOR SQLEXCEPTION
        BEGIN
            GET DIAGNOSTICS CONDITION 1
            @errno = MYSQL_ERRNO, @errmsg = MESSAGE_TEXT;
            SET p_error_message = CONCAT('Error ', @errno, ': ', @errmsg);
            SELECT 
                0 as grade_id,
                '' as name,
                '' as details,
                0 as sort_order,
                0 as stat_id,
                '' as stat_name,
                p_error_message as error_message;
        END;
        
        SET p_error_message = '';
        SET p_search = TRIM(COALESCE(p_search, ''));
        
        SELECT
            g.grade_id,
            g.name,
            g.details,
            g.sort_order,
            g.stat_id,
            s.name as stat_name,
            p_error_message as error_message
        FROM grades g
        LEFT JOIN stats s ON g.stat_id = s.stat_id
        WHERE g.stat_id != 0
          AND (p_grade_id = 0 OR g.grade_id = p_grade_id)
          AND (
                g.name LIKE CONCAT('%', p_search, '%')
                OR g.details LIKE CONCAT('%', p_search, '%')
                OR p_search = ''
              );
    END$$
    DELIMITER ;
     */
    public async Task<DataTable> CallProcedureQueryAsync(string storedProcedureName, Dictionary<string, object?>? args)
    {
        await using var conn = CreateConnection();
        await using var cmd = new MySqlCommand(storedProcedureName, conn);
        cmd.CommandType = CommandType.StoredProcedure;

        AddArgs(cmd, args);

        // Add output parameter for error if not present
        if (!cmd.Parameters.Contains("@p_error_message"))
        {
            cmd.Parameters.Add("@p_error_message", MySqlDbType.Text).Direction = ParameterDirection.Output;
        }

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);

        // Check for error in output parameter
        var error = cmd.Parameters["@p_error_message"].Value?.ToString();
        if (!string.IsNullOrWhiteSpace(error))
        {
            // You might want to log or handle this differently
            Console.WriteLine($"Procedure error: {error}");
        }

        return table;
    }

    /// <summary>
    /// Calls a MariaDB stored procedure and filters out error rows
    /// </summary>
    public async Task<DataTable> CallProcedureQueryParamAsync(string storedProcedureName, Dictionary<string, object?>? args)
    {
        var table = await CallProcedureQueryAsync(storedProcedureName, args);

        // Filter out rows with errors if there's an error_message column
        if (table.Columns.Contains("error_message"))
        {
            var filteredRows = table.AsEnumerable()
                .Where(row => string.IsNullOrWhiteSpace(row.Field<string>("error_message")))
                .ToList();

            if (filteredRows.Count != table.Rows.Count)
            {
                var filteredTable = table.Clone();
                foreach (var row in filteredRows)
                {
                    filteredTable.ImportRow(row);
                }
                return filteredTable;
            }
        }

        return table;
    }

    /// <summary>
    /// Executes a parameterized query directly (for backward compatibility)
    /// </summary>
    public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object?>? args)
    {
        await using var conn = CreateConnection();
        await using var cmd = new MySqlCommand(sql, conn);
        cmd.CommandType = CommandType.Text;

        AddArgs(cmd, args);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        var table = new DataTable();
        table.Load(reader);

        return table;
    }

    #endregion

    #region Stored Procedure - Multiple Result Sets

    /// <summary>
    /// Calls a MariaDB stored procedure that returns multiple result sets
    /// </summary>
    public async Task<List<DataTable>> CallProcedureMultipleResultsAsync(string storedProcedureName, Dictionary<string, object?>? args)
    {
        var results = new List<DataTable>();

        await using var conn = CreateConnection();
        await using var cmd = new MySqlCommand(storedProcedureName, conn);
        cmd.CommandType = CommandType.StoredProcedure;

        AddArgs(cmd, args);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();
        do
        {
            var table = new DataTable();
            table.Load(reader);
            results.Add(table);
        } while (!reader.IsClosed && reader.NextResult());

        return results;
    }

    #endregion

    #region Helpers

    public Dictionary<string, object?>? Args(params (string Name, object? Value)[] values)
    {
        return values.ToDictionary(v => v.Name, v => v.Value);
    }

    /// <summary>
    /// Creates a parameter dictionary with output parameter for procedures
    /// </summary>
    public Dictionary<string, object?> CreateProcedureArgs(params (string Name, object? Value, bool IsOutput)[] values)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var (Name, Value, IsOutput) in values)
        {
            dict[Name] = Value;
        }

        return dict;
    }

    #endregion
}

