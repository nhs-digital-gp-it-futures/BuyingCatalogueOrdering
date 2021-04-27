using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal sealed class OrderLookupOrganisationAuthorizationFilter : OrganisationAuthorizationFilter
    {
        internal const string DefaultParameterName = "callOffId";

        private readonly ApplicationDbContext dbContext;

        public OrderLookupOrganisationAuthorizationFilter(ApplicationDbContext dbContext) =>
            this.dbContext = dbContext;

        protected override string RouteParameterName => DefaultParameterName;

        protected override IEnumerable<string> ActionMethodParameterNames => new[] { DefaultParameterName, "order" };

        // ReSharper disable once RedundantOverriddenMember
        // There is a bug in the Roslyn analysers library that causes an ArgumentNullException to be thrown
        // when a class that implements IAsyncAuthorizationFilter does not have an OnAuthorizationAsync symbol
        // (because it derives from an abstract class as here). This in turn causes an AD0001 warning to be generated
        // that can't be removed or suppressed without disabling or relaxing the code analysis settings. As we treat
        // warnings as errors in the release build this causes the release build to fail. Rather than disable warnings
        // as errors or relax the code analysis settings, this redundant override is included to prevent the exception
        // in the analyser.
        // TODO: monitor Roslyn Analyser issue 4772.
        // Update to fixed version once available (may require use of NuGet version of the library) and remove
        // redundant override and revert the base method to non-virtual
        public override Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            return base.OnAuthorizationAsync(context);
        }

        protected override async Task<(string Id, IActionResult Result)> GetOrganisationId(string routeValue)
        {
            (bool success, CallOffId callOffId) = CallOffId.Parse(routeValue);
            if (!success)
                return (null, new NotFoundResult());

            var order = await dbContext.Order
                .Where(o => o.Id == callOffId.Id)
                .Select(o => new { OrderingPartyId = o.OrderingParty == null ? null : o.OrderingParty.Id.ToString() })
                .AsNoTracking()
                .SingleOrDefaultAsync();

            if (order is null)
                return (null, new NotFoundResult());

            return (order.OrderingPartyId, null);
        }
    }
}
