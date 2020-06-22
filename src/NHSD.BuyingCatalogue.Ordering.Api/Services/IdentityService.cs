using System;
using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserIdentity()
        {
            return _context.HttpContext.User.GetUserId();
        }

        public string GetUserName()
        {
            return _context.HttpContext.User.GetUserName();
        }
    }
}
