using System;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AccessTokenSteps
    {
        private readonly ScenarioContext _context;
        public AccessTokenSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"no user is logged in")]
        public void GivenNoAccessTokenIsAvailable()
        {
            _context[ScenarioContextKeys.AccessToken] = null;
        }

        [Given(@"the user is logged in with the (Buyer|Authority) role for organisation (.*)")]
        public void TheUserIsLoggedInWithRoleForOrganisation(string role, string organisationId)
        {
            var builder = new BearerTokenBuilder()
                .WithSigningCertificate(EmbeddedResourceReader.GetCertificate())
                .ForSubject("7B195137-6A59-4854-B118-62B39A3101EF")
                .WithClaim("client_id", "PasswordClient")
                .WithClaim("preferred_username", "BobSmith@email.com")
                .WithClaim("unique_name", "BobSmith@email.com")
                .WithClaim("given_name", "Bob")
                .WithClaim("family_name", "Smith")
                .WithClaim("name", "Bob Smith")
                .WithClaim("email", "BobSmith@email.com")
                .WithClaim("email_verified", "true")
                .WithClaim("primaryOrganisationId", organisationId)
                .WithClaim("organisationFunction", role);

            if (role.Equals("Buyer", StringComparison.InvariantCultureIgnoreCase))
            {
                builder = builder.WithClaim("Ordering", "Manage");
            }
            else if (role.Equals("Authority", StringComparison.InvariantCultureIgnoreCase))
            {
                builder = builder.WithClaim("Organisation", "Manage");
                builder = builder.WithClaim("Account", "Manage");
            }

            var token = builder.BuildToken();
            _context[ScenarioContextKeys.AccessToken] = token;
        }
    }
}
