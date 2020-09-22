namespace NHSD.BuyingCatalogue.Ordering.Domain
{
    public sealed class OdsOrganisation
    {
        public OdsOrganisation(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public string Code { get; }

        public string Name { get; }

        public void Deconstruct(out string code, out string name)
        {
            code = Code;
            name = Name;
        }
    }
}
