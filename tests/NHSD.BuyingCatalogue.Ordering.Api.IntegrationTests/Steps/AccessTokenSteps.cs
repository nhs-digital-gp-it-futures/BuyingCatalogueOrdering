﻿using System;
using System.Security.Claims;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps.Support;
using NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils;
using TechTalk.SpecFlow;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Steps
{
    [Binding]
    internal sealed class AccessTokenSteps
    {
        private readonly ScenarioContext context;
        private readonly Settings settings;

        public AccessTokenSteps(ScenarioContext context, Settings settings)
        {
            this.context = context;
            this.settings = settings;
        }

        [Given(@"no user is logged in")]
        public void GivenNoAccessTokenIsAvailable()
        {
            context[ScenarioContextKeys.AccessToken] = null;
        }

        [Given(@"the user is logged in with the (Buyer|Authority|Read-only Buyer) role for organisation (.*)")]
        public void TheUserIsLoggedInWithRoleForOrganisation(string role, string organisationId)
        {
            var builder = new BearerTokenBuilder()
                .WithSigningCertificate(EmbeddedResourceReader.GetCertificate())
                .IssuedBy(settings.Authority)
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
                .WithClaim("organisationFunction", role)
                .WithClaim(ClaimTypes.Name, "Test User")
                .WithClaim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString());

            if (role.Equals("Read-only Buyer", StringComparison.OrdinalIgnoreCase))
            {
                builder.WithClaim("Ordering", "view");
            }

            if (role.Equals("Buyer", StringComparison.OrdinalIgnoreCase))
            {
                builder = builder.WithClaim("Ordering", "Manage");
            }
            else if (role.Equals("Authority", StringComparison.OrdinalIgnoreCase))
            {
                builder = builder.WithClaim("Organisation", "Manage");
                builder = builder.WithClaim("Account", "Manage");
            }

            var token = builder.BuildToken();
            context[ScenarioContextKeys.AccessToken] = token;
        }
    }
}
