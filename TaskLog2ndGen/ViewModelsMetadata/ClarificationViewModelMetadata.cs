using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.ViewModels
{
    [MetadataType(typeof(ClarificationViewModelMetadata))]
    public partial class ClarificationViewModel
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for clarification viewmodel
    /// </summary>
    public class ClarificationViewModelMetadata
    {
        public int taskId { get; set; }
        [Display(Name = "To")]
        [Required(ErrorMessage = "To is required.")]
        public string to { get; set; }

        [Display(Name = "CC")]
        public string cc { get; set; }

        [Display(Name = "From")]
        [Required(ErrorMessage = "From is required.")]
        public string from { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Subject is required.")]
        public string subject { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "Body is required.")]
        public string body { get; set; }
    }
}