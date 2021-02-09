using System.Collections.Generic;
using System.Security.Claims;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Authorization
{
    internal sealed class ClaimsPrincipalBuilder
    {
        private readonly List<Claim> claims = new();

        private ClaimsPrincipalBuilder()
        {
        }

        internal static ClaimsPrincipalBuilder Create() => new();

        internal ClaimsPrincipalBuilder WithClaim(string type, string value = "")
        {
            claims.Add(new Claim(type, value));
            return this;
        }

        internal ClaimsPrincipal Build() => new(new ClaimsIdentity(claims, "mock"));
    }
}
