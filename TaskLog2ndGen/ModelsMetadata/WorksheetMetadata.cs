using System;
using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    /// <summary>
    /// Entity for worksheet table
    /// </summary>
    [MetadataType(typeof(WorksheetMetadata))]
    public partial class Worksheet
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for worksheet entity
    /// </summary>
    public class WorksheetMetadata
    {
        [Display(Name = "Employee Assigned")]
        [Required(ErrorMessage = "Employee assigned is required.")]
        public int employee { get; set; }

        [Display(Name = "Task")]
        public int task { get; set; }

        [Display(Name = "Status")]
        public string worksheetStatus { get; set; }

        [Display(Name = "Date Assigned")]
        public System.DateTime dateAssigned { get; set; }

        [Display(Name = "Notes")]
        [Required(ErrorMessage = "Notes are required.")]
        [StringLength(255, ErrorMessage = "Notes cannot have more than 255 characters.")]
        public string notes { get; set; }

        [Display(Name = "Time Spent")]
        [Required(ErrorMessage = "Time spent is required.")]
        [Range(0.00, 999.99, ErrorMessage = "Time spent must be between 0.00 and 999.99.")]
        public decimal timeSpent { get; set; }

        [Display(Name = "Overtime")]
        public Nullable<bool> overtime { get; set; }

        [Display(Name = "On Call")]
        public Nullable<bool> onCall { get; set; }

        [Display(Name = "Links")]
        [StringLength(255, ErrorMessage = "Links cannot have more than 255 characters.")]
        public string links { get; set; }
    }
}