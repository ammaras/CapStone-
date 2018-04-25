using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.ViewModels
{
    [MetadataType(typeof(TaskViewModelMetadata))]
    public partial class TaskReferenceViewModel
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for task viewmodel
    /// </summary>
    public class TaskViewModelMetadata
    {
        [Display(Name = "Primary Contact")]
        [Required(ErrorMessage = "Primary contact is required.")]
        public int primaryContact { get; set; }

        [Display(Name = "Secondary Contact")]
        public int? secondaryContact { get; set; }

        [Display(Name = "Date Logged")]
        [Required(ErrorMessage = "Date logged is required.")]
        public System.DateTime dateLogged { get; set; }

        [Display(Name = "Date Submitted")]
        [Required(ErrorMessage = "Date submitted is required.")]
        public System.DateTime dateSubmmited { get; set; }

        [Display(Name = "Service Team")]
        [Required(ErrorMessage = "Service team is required.")]
        public int serviceTeam { get; set; }

        [Display(Name = "Service Group")]
        [Required(ErrorMessage = "Service group is required.")]
        public int serviceGroup { get; set; }

        [Display(Name = "Platform")]
        [Required(ErrorMessage = "Platform is required.")]
        public string platform { get; set; }

        [Display(Name = "Urgency")]
        [Required(ErrorMessage = "Urgency is required.")]
        public string urgency { get; set; }

        [Display(Name = "Business Unit")]
        [Required(ErrorMessage = "Business unit is required.")]
        public int businessUnit { get; set; }

        [Display(Name = "Environment")]
        [Required(ErrorMessage = "Environment is required.")]
        public string environment { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Category is required.")]
        public string category { get; set; }

        [Display(Name = "Application")]
        [Required(ErrorMessage = "Application is required.")]
        public int application { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot have more than 50 characters.")]
        public string title { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, ErrorMessage = "Description cannot have more than 255 characters.")]
        public string description { get; set; }

        [Display(Name = "High Level Estimate")]
        public string highLevelEstimate { get; set; }

        [Display(Name = "Links")]
        [StringLength(255, ErrorMessage = "Links cannot have more than 255 characters.")]
        public string links { get; set; }

        [Display(Name = "Task Status")]
        public string taskStatus { get; set; }
    }
}