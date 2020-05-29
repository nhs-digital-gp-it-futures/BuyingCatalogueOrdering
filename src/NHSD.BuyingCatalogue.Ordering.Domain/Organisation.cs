using System;
using System.Collections.Generic;
using System.Text;

namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class Organisation
    {
        public string Name { get; set; }
        public string OdsCode { get; set; }
        public Address Adress { get; set; }
    }
}
