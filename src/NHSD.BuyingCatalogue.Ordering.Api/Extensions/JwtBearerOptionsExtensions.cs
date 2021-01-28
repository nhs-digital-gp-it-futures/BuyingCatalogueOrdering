using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace NHSD.BuyingCatalogue.Ordering.Api.Extensions
{
    public static class JwtBearerOptionsExtensions
    {
        private const string OpenIdConfiguration = @"{
            ""issuer"": ""http://host.docker.internal:8070/identity"",
            ""jwks_uri"": ""http://localhost:8070/identity/.well-known/openid-configuration/jwks"",
            ""authorization_endpoint"": ""http://localhost:8070/identity/connect/authorize"",
            ""token_endpoint"": ""http://localhost:8070/identity/connect/token"",
            ""userinfo_endpoint"": ""http://localhost:8070/identity/connect/userinfo"",
            ""end_session_endpoint"": ""http://localhost:8070/identity/connect/endsession"",
            ""check_session_iframe"": ""http://localhost:8070/identity/connect/checksession"",
            ""revocation_endpoint"": ""http://localhost:8070/identity/connect/revocation"",
            ""introspection_endpoint"": ""http://localhost:8070/identity/connect/introspect"",
            ""device_authorization_endpoint"": ""http://localhost:8070/identity/connect/deviceauthorization""
        }";

        private const string WebKeySet = @"{ ""keys"": [
            {
                ""kty"": ""RSA"",
                ""use"": ""sig"",
                ""kid"": ""B2C4330609F2EB8D91247E256D587058DBC95BFD"",
                ""x5t"": ""ssQzBgny642RJH4lbVhwWNvJW_0"",
                ""e"": ""AQAB"",
                ""n"": ""zewb-3SZNtLNL76fSWYmgFtdUv-6C2mqZRmpp2gOuo5XyUyC7Uz6jJgPl4eCdARQzu2FKoarFtDdsdFTsow2w12FU-wE3u_vhMr0EaWQZeAPUr2LLNIF9EJjbvep4DJ9a__9mp7SNVVBCK7ntH0DBJ7FMJdJ_2OBUFICoFU_QRfEBpLBn--vkAvPCNci0G87kcFD6Nz6iYF5xr4QkAt8INZE_oTVVTPBseZpfOKpaRZvq6CTSw_Qv3GFoVCZHUxRsBKSHoGyV8HtyurS-5RJeFZDDZAA0ea_NS6fn3Qj93dQQCNKGQcmvP8ya-CW704P2LrOSTz5wQLLliCs3M4oZw"",
                ""x5c"": [
                    ""MIIDwzCCAqugAwIBAgIUPu2BmC1x8IQOB2paKNwkNgRmkJ0wDQYJKoZIhvcNAQELBQAwcTELMAkGA1UEBhMCR0IxDTALBgNVBAgMBFlvbG8xDDAKBgNVBAcMA0JvYjENMAsGA1UECgwERnJlZDEOMAwGA1UECwwFSmltbXkxEDAOBgNVBAMMB01ZIG5hbWUxFDASBgkqhkiG9w0BCQEWBWFAYi5jMB4XDTIwMDUxMjEzMzI1OVoXDTIxMDUxMjEzMzI1OVowcTELMAkGA1UEBhMCR0IxDTALBgNVBAgMBFlvbG8xDDAKBgNVBAcMA0JvYjENMAsGA1UECgwERnJlZDEOMAwGA1UECwwFSmltbXkxEDAOBgNVBAMMB01ZIG5hbWUxFDASBgkqhkiG9w0BCQEWBWFAYi5jMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzewb+3SZNtLNL76fSWYmgFtdUv+6C2mqZRmpp2gOuo5XyUyC7Uz6jJgPl4eCdARQzu2FKoarFtDdsdFTsow2w12FU+wE3u/vhMr0EaWQZeAPUr2LLNIF9EJjbvep4DJ9a//9mp7SNVVBCK7ntH0DBJ7FMJdJ/2OBUFICoFU/QRfEBpLBn++vkAvPCNci0G87kcFD6Nz6iYF5xr4QkAt8INZE/oTVVTPBseZpfOKpaRZvq6CTSw/Qv3GFoVCZHUxRsBKSHoGyV8HtyurS+5RJeFZDDZAA0ea/NS6fn3Qj93dQQCNKGQcmvP8ya+CW704P2LrOSTz5wQLLliCs3M4oZwIDAQABo1MwUTAdBgNVHQ4EFgQUlRDmWazTYbXkYDOK7EvRdQfiWvswHwYDVR0jBBgwFoAUlRDmWazTYbXkYDOK7EvRdQfiWvswDwYDVR0TAQH/BAUwAwEB/zANBgkqhkiG9w0BAQsFAAOCAQEAfbTaLB5Gw1at/h9xI+sVALnoK3ACWvi/5fm10nI67b/Wf20JOyI7afnFOksc4Y6vdSggaBKUC04UIEQsO1r6Jd6kpAPiWiLbnSxWjxdpXH3RZz0f1tnZXnmPlUQQVzgWEENJJsIfW0WZj1MtMJ1nmxrw7dyUTSW+ak7pFE/eHzXR8nicprSpGdKRTGEgwK81cSN+Nh5fsa6rjvQ6a4RAvsySZ0AkLnk7/uFjv8B8iK+VTQlqAfJ2J7FPmbRv5Bk4bp7d3eZ/g8/XbsnsLpPB6T/3KymHqtfQvhDKQzZMRCZLBePpf2NQSpOMwQPmB+ATvZnAjknzg2I62fCqzKFMog==""
                ],
                ""alg"": ""RS256""
            }
        ]}";

        public static void BypassIdentity(this JwtBearerOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            options.Configuration = new OpenIdConnectConfiguration(OpenIdConfiguration);
            options.TokenValidationParameters.IssuerSigningKeys = new JsonWebKeySet(WebKeySet).GetSigningKeys();
        }
    }
}
