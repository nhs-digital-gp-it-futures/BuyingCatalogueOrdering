using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.BuyingCatalogue.Ordering.Application.Persistence;
using NHSD.BuyingCatalogue.Ordering.Domain;
using NHSD.BuyingCatalogue.Ordering.Persistence.Data;

namespace NHSD.BuyingCatalogue.Ordering.Persistence.Repositories
{
    public sealed class ServiceRecipientRepository : IServiceRecipientRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRecipientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRecipient>> ListServiceRecipientsByOrderIdAsync(string orderId)
        {
            return await _context.ServiceRecipient
                .Include(x => x.Order)
                .Where(s => s.Order.OrderId == orderId).ToListAsync();
        }
    }
}
