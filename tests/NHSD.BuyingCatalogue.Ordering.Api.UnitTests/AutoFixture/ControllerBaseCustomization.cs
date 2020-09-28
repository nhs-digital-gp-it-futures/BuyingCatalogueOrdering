using System;
using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NHSD.BuyingCatalogue.Ordering.Api.UnitTests.AutoFixture
{
    internal sealed class ControllerBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(
                new FilteringSpecimenBuilder(
                    new Postprocessor(
                        new MethodInvoker(new ModestConstructorQuery()),
                        new ControllerBaseSpecimenCommand()),
                    new ControllerBaseRequestSpecification()));
        }

        private class ControllerBaseSpecimenCommand : ISpecimenCommand
        {
            public void Execute(object specimen, ISpecimenContext context)
            {
                if (specimen == null)
                    throw new ArgumentNullException(nameof(specimen));

                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (specimen is ControllerBase controller)
                {
                    controller.ControllerContext = new ControllerContext
                    {
                        HttpContext = (HttpContext)context.Resolve(typeof(HttpContext))
                    };
                }
                else
                {
                    throw new ArgumentException("The specimen must be an instance of ControllerBase", nameof(specimen));
                }
            }
        }

        private class ControllerBaseRequestSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request) =>
                request is Type type && typeof(ControllerBase).IsAssignableFrom(type);
        }
    }
}
