using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    public sealed class BearerTokenBuilder
    {
        private readonly List<Claim> claims = new();
        private readonly JwtSecurityTokenHandler securityTokenHandler = new();
        private X509Certificate2 signingCertificate;
        private string issuer;
        private string audience = "Ordering";
        private TimeSpan life = TimeSpan.FromHours(1);
        private DateTime notBefore = DateTime.UtcNow;

        public BearerTokenBuilder IssuedBy(string issuedBy)
        {
            if (string.IsNullOrEmpty(issuedBy))
            {
                throw new ArgumentException("Issued by cannot be null or empty", nameof(issuedBy));
            }

            issuer = issuedBy;

            return this;
        }

        public BearerTokenBuilder ForAudience(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Audience cannot be null or empty", nameof(value));
            }

            audience = value;

            return this;
        }

        public BearerTokenBuilder ForSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("Subject cannot be null or empty", nameof(subject));
            }

            if (claims.FirstOrDefault(claim => claim.Type == "sub") == null)
            {
                claims.Add(new Claim("sub", subject));
            }

            return this;
        }

        public BearerTokenBuilder WithSigningCertificate(X509Certificate2 certificate)
        {
            signingCertificate = certificate ?? throw new ArgumentException(
                "Certificate cannot be null or empty",
                nameof(certificate));

            return this;
        }

        public BearerTokenBuilder WithClaim(string claimType, string value)
        {
            if (string.IsNullOrEmpty(claimType))
            {
                throw new ArgumentException("Claim type cannot be null or empty", nameof(claimType));
            }

            value ??= string.Empty;

            claims.Add(new Claim(claimType, value));

            return this;
        }

        public BearerTokenBuilder WithLifetime(TimeSpan lifetime)
        {
            life = lifetime;
            return this;
        }

        public BearerTokenBuilder NotBefore(DateTime notBeforeDate)
        {
            notBefore = notBeforeDate;
            return this;
        }

        public string BuildToken()
        {
            if (signingCertificate == null)
            {
                throw new InvalidOperationException(
                    "You must specify an X509 certificate to use for signing the JWT Token");
            }

            var signingCredentials =
                new SigningCredentials(new X509SecurityKey(signingCertificate), SecurityAlgorithms.RsaSha256);

            var notBeforeDate = notBefore;
            var expires = notBeforeDate.Add(life);

            var identity = new ClaimsIdentity(claims);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = audience,
                Issuer = issuer,
                NotBefore = notBeforeDate,
                Expires = expires,
                SigningCredentials = signingCredentials,
                Subject = identity,
            };

            var token = securityTokenHandler.CreateToken(securityTokenDescriptor);

            var encodedAccessToken = securityTokenHandler.WriteToken(token);

            return encodedAccessToken;
        }
    }
}
