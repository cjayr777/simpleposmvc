using System.Globalization;

namespace Proj.Util;

public class OpResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public static OpResult Ok(string message = "")
    {
        OpResult result = new OpResult();
        result.IsSuccess = true;
        result.Message = message;

        return result;
    }

    public static OpResult Fail(string message)
    {
        OpResult result = new OpResult();
        result.IsSuccess = false;
        result.Message = message;

        return result;
    }

}

public class DataResult<T> : OpResult
{
    public T? Data { get; set; }

    public static DataResult<T> Ok(T data, string message = "")
    {
        DataResult<T> result = new DataResult<T>();
        result.Data = data;
        result.IsSuccess = true;
        result.Message = message;

        return result;
    }

    public static DataResult<T> Fail(T data = default, string message = "")
    {
        DataResult<T> result = new DataResult<T>();
        result.IsSuccess = false;
        result.Message = message;
        result.Data = data;

        return result;
    }

}