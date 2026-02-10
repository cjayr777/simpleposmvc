using System.Data;

namespace Proj.Util;

public static class Turn
{
    public static int GetSafeInt(Dictionary<string, object> dict, string key)
    {
        int result = 
            dict.ContainsKey(key) && dict[key] != DBNull.Value 
            ? Convert.ToInt32(dict[key]) 
            : 0;

        return result;
    }

    public static int GetSafeInt(DataRow row, string columnName)
    {
        var result = row[columnName] is DBNull 
            ? 0 
            : Convert.ToInt32(row[columnName]);

        return result;
    }

    public static string GetSafeString(Dictionary<string, object> dict, string key)
    {
        string result = dict.ContainsKey(key) && dict[key] != DBNull.Value 
            ? Convert.ToString(dict[key]) 
            : "";

        return result;
    }

    public static string GetSafeString(DataRow row, string columnName)
    {
        var result = row[columnName] as string ?? string.Empty;

        return result;
    }

    public static int Int(object p)
    {
        try
        {
            int x = Convert.ToInt32(p);

            return x;
        }
        catch
        {
            return 0;
        }
    }

    public static string String(object p)
    {
        try
        {
            string x = Convert.ToString(p) ?? "";

            return x;
        }
        catch
        {
            return "";
        }
    }
    
}
