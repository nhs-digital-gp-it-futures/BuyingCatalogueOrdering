namespace NHSD.BuyingCatalogue.Ordering.Domain.Orders
{
    public static class OrderErrors
    {
        public static ErrorDetails OrderDescriptionRequired()
        {
            return new ErrorDetails("OrderDescriptionRequired", nameof(Order.Description));
        }

        public static ErrorDetails OrderDescriptionTooLong()
        {
            return new ErrorDetails("OrderDescriptionTooLong", nameof(Order.Description));
        }

        public static ErrorDetails OrderOrganisationIdRequired()
        {
            return new ErrorDetails("OrganisationIdRequired", nameof(Order.OrganisationId));
        }

        public static ErrorDetails ContactNameRequired(string fieldName)
        {
            return new ErrorDetails(fieldName+"Required", fieldName);
        }

        public static ErrorDetails ContactNameTooLong(string fieldName)
        {
            return new ErrorDetails(fieldName + "TooLong", fieldName);
        }

        public static ErrorDetails ContactEmailAddressRequired()
        {
            //TODO JSON Contract field nameof email address to be returned
            return new ErrorDetails("EmailAddressRequired", nameof(Contact.Email));
        }

        public static ErrorDetails ContactEmailAddressTooLong()
        {
            //TODO JSON Contract field nameof email address to be returned
            return new ErrorDetails("EmailAddressTooLong", nameof(Contact.Email));
        }
        
        public static ErrorDetails ContactEmailAddressInvalidFormat()
        {
            //TODO JSON Contract field nameof email address to be returned
            return new ErrorDetails("EmailAddressInvalidFormat", nameof(Contact.Email));
        }

        public static ErrorDetails ContactTelephoneNumberRequired()
        {
            //TODO JSON Contract field nameof Telephone Number address to be returned
            return new ErrorDetails("TelephoneNumberRequired", nameof(Contact.Phone));
        }

        public static ErrorDetails ContactTelephoneNumberTooLong()
        {
            //TODO JSON Contract field nameof Telephone Number to be returned
            return new ErrorDetails("TelephoneNumberTooLong", nameof(Contact.Phone));
        }

    }
}
