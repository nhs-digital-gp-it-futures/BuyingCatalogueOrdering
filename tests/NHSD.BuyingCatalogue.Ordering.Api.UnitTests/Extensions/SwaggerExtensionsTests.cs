using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Extensions;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SwaggerExtensionsTests
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        public static void AddSwaggerDocumentation_ParameterIsNull_ThrowsArgumentNullException(bool hasServiceCollection, bool hasConf)
        {
            var serviceCollection = new Mock<IServiceCollection>();
            var configuration = new Mock<IConfiguration>();

            Assert.Throws<ArgumentNullException>(() =>
                (hasServiceCollection ? serviceCollection.Object : null).AddSwaggerDocumentation(hasConf ? configuration.Object : null));
        }

        [Test]
        public static void UseSwaggerDocumentation_ParameterIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SwaggerExtensions.UseSwaggerDocumentation(null));
        }
    }
}
