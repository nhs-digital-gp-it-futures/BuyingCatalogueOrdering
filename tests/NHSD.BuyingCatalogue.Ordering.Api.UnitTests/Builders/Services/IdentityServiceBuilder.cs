using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Services;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Builders.Services
{
    internal sealed class IdentityServiceBuilder
    {
        private IHttpContextAccessor _httpContextAccessor;

        private IdentityServiceBuilder()
        {
            _httpContextAccessor = Mock.Of<IHttpContextAccessor>();
        }

        public static IdentityServiceBuilder Create() => new IdentityServiceBuilder();

        public IdentityServiceBuilder WithHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            return this;
        }

        public IdentityService Build() => new IdentityService(_httpContextAccessor);
    }
}
