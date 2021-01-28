using System.Security.Cryptography.X509Certificates;

namespace NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests.Utils
{
    internal sealed class EmbeddedResourceReader
    {
        private const string EmbeddedResourceQualifier = "NHSD.BuyingCatalogue.Ordering.Api.IntegrationTests";

        public static X509Certificate2 GetCertificate()
        {
            var resourceName = $"{EmbeddedResourceQualifier}.IdentitySigningKey.pfx";
            using var certificateStream =
                typeof(EmbeddedResourceReader).Assembly.GetManifestResourceStream(resourceName);

            if (certificateStream is null)
            {
                return null;
            }

            var rawBytes = new byte[certificateStream.Length];
            for (var i = 0; i < certificateStream.Length; i++)
            {
                rawBytes[i] = (byte)certificateStream.ReadByte();
            }

            return new X509Certificate2(rawBytes, "12345", X509KeyStorageFlags.UserKeySet);
        }
    }
}
