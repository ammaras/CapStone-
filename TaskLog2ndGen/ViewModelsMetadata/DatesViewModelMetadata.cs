using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.ViewModels
{
    [MetadataType(typeof(DatesViewModelMetadata))]
    public partial class DatesViewModel
    {

    }

    public class DatesViewModelMetadata
    {
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start date is required.")]
        public System.DateTime startDate { get; set; }

        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End date is required.")]
        public System.DateTime endDate { get; set; }
    }
}