using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    /// <summary>
    /// Entity for reference table
    /// </summary>
    [MetadataType(typeof(ReferenceMetadata))]
    public partial class Reference
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for reference entity
    /// </summary>
    public class ReferenceMetadata
    {
        [Display(Name = "Reference Number")]
        public string referenceNo { get; set; }

        [Display(Name = "Reference Type")]
        public string referenceType { get; set; }
    }
}