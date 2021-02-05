using System;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthorizeOrganisationAttribute : Attribute
    {
    }
}
