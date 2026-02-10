using Microsoft.AspNetCore.DataProtection;

namespace PointOfSaleSimpleVersionMvc.ViewHelpers;

public class IdProtector
{
    private readonly IDataProtector protector;

    protected IdProtector(IDataProtectionProvider provider, string purpose)
    {
        protector = provider.CreateProtector(purpose);
    }

    public string Protect(int id)
        => protector.Protect(id.ToString());

    public int Unprotect(string protectedId)
        => int.Parse(protector.Unprotect(protectedId));
}

public sealed class IdProtector<T> : IdProtector
{
    public IdProtector(IDataProtectionProvider provider)
        : base(provider, $"SimplePointOfSaleMvc.{typeof(T).Name}")
    {
    }
}
