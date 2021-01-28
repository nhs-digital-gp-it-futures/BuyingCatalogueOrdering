﻿using System;
using Microsoft.AspNetCore.Http;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NHSD.BuyingCatalogue.Ordering.Application.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services
{
    internal sealed class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor context;

        public IdentityService(IHttpContextAccessor context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserIdentity()
        {
            if (context.HttpContext is null)
                throw new InvalidOperationException();

            return context.HttpContext.User.GetUserId();
        }

        public string GetUserName()
        {
            if (context.HttpContext is null)
                throw new InvalidOperationException();

            return context.HttpContext.User.GetUserName();
        }

        public IdentityUser GetUserInfo() => new(GetUserIdentity(), GetUserName());
    }
}
