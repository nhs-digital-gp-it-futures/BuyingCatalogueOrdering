using System.Threading.Tasks;
using NHSD.BuyingCatalogue.Ordering.Domain;

namespace NHSD.BuyingCatalogue.Ordering.Contracts
{
    public interface ISupplierSectionService
    {
        Task<Order> GetOrder(CallOffId callOffId);

        Task SetSupplierSection(Order order, Supplier supplier, Contact contact);
    }
}
