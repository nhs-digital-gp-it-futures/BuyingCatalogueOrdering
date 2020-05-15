using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Common.Results;

namespace NHSD.BuyingCatalogue.Ordering.Api.Services.CreateOrder
{
    public interface ICreateOrderService
    {
        Task<Result<string>> CreateAsync(CreateOrderRequest createBuyerRequest);
    }
}
