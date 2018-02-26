using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {

    }


    public class EmployeeMetadata
    {
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "First Name")]
        public string firstName { get; set; }

        [Display(Name = "E-Mail")]
        public string email { get; set; }

        [Display(Name = "Phone Number")]
        public string phone { get; set; }

        [Display(Name = "Extension")]
        public string extension { get; set; }
    }
}