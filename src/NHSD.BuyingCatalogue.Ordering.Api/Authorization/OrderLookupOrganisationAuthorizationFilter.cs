using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;

namespace NHSD.BuyingCatalogue.Ordering.Api.Authorization
{
    internal sealed class OrderLookupOrganisationAuthorizationFilter : OrganisationAuthorizationFilter
    {
        internal const string DefaultParameterName = "orderId";

        private readonly IOrderRepository orderRepository;

        public OrderLookupOrganisationAuthorizationFilter(IOrderRepository orderRepository) =>
            this.orderRepository = orderRepository;

        protected override string ParameterName { get; } = DefaultParameterName;

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
            var order = await orderRepository.GetOrderByIdAsync(routeValue, q => q.WithoutTracking());
            if (order is null)
                return (null, new NotFoundResult());

            return (order.OrganisationId.ToString(), null);
        }
    }
}
