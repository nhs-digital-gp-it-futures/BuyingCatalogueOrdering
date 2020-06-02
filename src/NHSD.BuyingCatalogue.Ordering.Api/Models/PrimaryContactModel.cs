using System.ComponentModel.DataAnnotations;

namespace NHSD.BuyingCatalogue.Ordering.Api.Models
{
    public sealed class PrimaryContactModel
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        [MaxLength(100, ErrorMessage = "FirstNameTooLong")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastNameRequired")]
        [MaxLength(100, ErrorMessage = "LastNameTooLong")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "EmailAddressRequired")]
        [EmailAddress(ErrorMessage = "EmailAddressInvalidFormat")]
        [MaxLength(256, ErrorMessage = "EmailAddressTooLong")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "TelephoneNumberRequired")]
        [MaxLength(35, ErrorMessage = "TelephoneNumberTooLong")]
        public string TelephoneNumber { get; set; }
    }
}
