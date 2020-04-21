using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.Logging;
using NUnit.Framework;
using Serilog.Events;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests
{
    [TestFixture]
    internal sealed class SerilogRequestLoggingOptionsTests
    {
        [Test]
        public void GetLevel_Exception_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, new InvalidOperationException())
                .Should().Be(LogEventLevel.Error);
        }

        [Test]
        public void GetLevel_NullHttpContext_ReturnsError()
        {
            SerilogRequestLoggingOptions.GetLevel(null, 0, null)
                .Should().Be(LogEventLevel.Error);
        }

        [TestCase(null, StatusCodes.Status200OK, LogEventLevel.Information)]
        [TestCase(null, 498, LogEventLevel.Information)]
        [TestCase(null, 499, LogEventLevel.Information)]
        [TestCase(null, StatusCodes.Status500InternalServerError, LogEventLevel.Error)]
        [TestCase(null, StatusCodes.Status501NotImplemented, LogEventLevel.Error)]
        [TestCase("Not a health check", StatusCodes.Status200OK, LogEventLevel.Information)]
        [TestCase(SerilogRequestLoggingOptions.HealthCheckEndpointDisplayName, StatusCodes.Status200OK, LogEventLevel.Verbose)]
        public void IsHealthCheck_ReturnsExpectedValue(
            string endpointDisplayName,
            int statusCode,
            LogEventLevel expectedLevel)
        {
            var featureCollection = new FeatureCollection();
            var endpoint = new Endpoint(null, null, endpointDisplayName);
            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Features).Returns(featureCollection);
            mockContext.Setup(c => c.Response.StatusCode).Returns(statusCode);

            var context = mockContext.Object;
            context.SetEndpoint(endpoint);

            SerilogRequestLoggingOptions.GetLevel(context, 0, null)
                .Should().Be(expectedLevel);
        }
    }
}
