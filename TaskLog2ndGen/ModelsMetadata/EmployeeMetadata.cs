using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    /// <summary>
    /// Entity for employee table
    /// </summary>
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        public string fullName
        {
            get
            {
                return lastName + ", " + firstName;
            }
        }
    }

    /// <summary>
    /// Metadata class with validation annotations for employee entity
    /// </summary>
    public class EmployeeMetadata
    {
        [Display(Name = "Service Team")]
        [Required(ErrorMessage = "Service team is required.")]
        public int team { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot have more than 50 characters.")]
        public string lastName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot have more than 50 characters.")]
        [Required(ErrorMessage = "First name is required.")]
        public string firstName { get; set; }

        [Display(Name = "E-Mail")]
        [Required(ErrorMessage = "E-mail is required.")]
        [StringLength(50, ErrorMessage = "E-mail cannot have more than 50 characters.")]
        [RegularExpression("[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "E-mail must be in a valid format.")]
        public string email { get; set; }

        [Display(Name = "Description")]
        [StringLength(255, ErrorMessage = "Description cannot have more than 255 characters.")]
        public string description { get; set; }

        [Display(Name = "Last Changed")]
        public System.DateTime lastChanged { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot have more than 50 characters.")]
        public string middleName { get; set; }

        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(14, ErrorMessage = "Phone number cannot have more than 14 characters.")]
        [RegularExpression("\\D*([2-9]\\d{2})(\\D*)([2-9]\\d{2})(\\D*)(\\d{4})\\D*", ErrorMessage = "Phone number must be in a valid canadian format.")]
        public string phone { get; set; }

        [Display(Name = "Extension")]
        [Required(ErrorMessage = "Extension is required.")]
        [RegularExpression("^[0-9]{5}$", ErrorMessage = "Extension must be five digits.")]
        public string extension { get; set; }
    }
}