using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    [MetadataType(typeof(ReferenceMetadata))]
    public partial class Reference
    {

    }

    public class ReferenceMetadata
    {
        [Display(Name = "Reference Number")]
        public string referenceNo { get; set; }

        [Display(Name = "Reference Type")]
        public string referenceType { get; set; }
    }
}