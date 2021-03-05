using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Moq;
using NHSD.BuyingCatalogue.Ordering.Api.ModelBinding;
using NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;
using NUnit.Framework;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.ModelBinding
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [SuppressMessage("ReSharper", "NUnit.MethodWithParametersAndTestAttribute", Justification = "False positive")]
    internal static class OrderModelBinderTests
    {
        [Test]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderModelBinder).GetConstructors();

            assertion.Verify(constructors);
        }

        [Test]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_NullOrWhiteSpaceModelName_UsesDefault), null)]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_NullOrWhiteSpaceModelName_UsesDefault), "")]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_NullOrWhiteSpaceModelName_UsesDefault), "\t")]
        public static async Task BindModelAsync_NullOrWhiteSpaceModelName_UsesDefault(
            string modelName,
            Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ModelName).Returns(modelName);
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(ValueProviderResult.None);

            await modelBinder.BindModelAsync(contextMock.Object);

            valueProviderMock.Verify(v => v.GetValue(It.Is<string>(s => s == OrderModelBinder.DefaultModelName)));
        }

        [Test]
        [InMemoryDbAutoData(nameof(BindModelAsync_NoValue_ResultIsFailed))]
        public static async Task BindModelAsync_NoValue_ResultIsFailed(
            Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock.Setup(v => v.GetValue(It.IsAny<string>())).Returns(ValueProviderResult.None);

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Test]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_ValueIsNullOrWhiteSpace_ResultIsFailed), null)]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_ValueIsNullOrWhiteSpace_ResultIsFailed), "")]
        [InMemoryDbInlineAutoData(nameof(BindModelAsync_ValueIsNullOrWhiteSpace_ResultIsFailed), "\t")]
        public static async Task BindModelAsync_ValueIsNullOrWhiteSpace_ResultIsFailed(
            string value,
            Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock
                .Setup(v => v.GetValue(It.IsAny<string>()))
                .Returns(new ValueProviderResult(new StringValues(value)));

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Test]
        [InMemoryDbAutoData(nameof(BindModelAsync_InvalidCallOffId_AddsModelError))]
        public static async Task BindModelAsync_InvalidCallOffId_AddsModelError(
            [Frozen] Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock
                .Setup(v => v.GetValue(It.IsAny<string>()))
                .Returns(new ValueProviderResult(new StringValues("NotCallOffId")));

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.ModelState.Should().HaveCount(1);
            context.ModelState.Keys.Should().Contain(OrderModelBinder.DefaultModelName);
        }

        [Test]
        [InMemoryDbAutoData(nameof(BindModelAsync_InvalidCallOffId_ResultIsFailed))]
        public static async Task BindModelAsync_InvalidCallOffId_ResultIsFailed(
            Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock
                .Setup(v => v.GetValue(It.IsAny<string>()))
                .Returns(new ValueProviderResult(new StringValues("NotCallOffId")));

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Failed());
        }

        [Test]
        [InMemoryDbAutoData(nameof(BindModelAsync_ReturnsSuccessWithOrder))]
        public static async Task BindModelAsync_ReturnsSuccessWithOrder(
            [Frozen] ApplicationDbContext dbContext,
            [Frozen] CallOffId callOffId,
            Order order,
            Mock<IValueProvider> valueProviderMock,
            Mock<ModelBindingContext> contextMock,
            OrderModelBinder modelBinder)
        {
            dbContext.Order.Add(order);
            await dbContext.SaveChangesAsync();

            contextMock.SetupAllProperties();
            contextMock.Setup(c => c.ValueProvider).Returns(valueProviderMock.Object);
            valueProviderMock
                .Setup(v => v.GetValue(It.IsAny<string>()))
                .Returns(new ValueProviderResult(new StringValues(callOffId.ToString())));

            var context = contextMock.Object;

            await modelBinder.BindModelAsync(context);

            context.Result.Should().Be(ModelBindingResult.Success(order));
        }
    }
}
