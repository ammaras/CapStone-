using System.ComponentModel.DataAnnotations;

namespace TaskLog2ndGen.Models
{
    /// <summary>
    /// Entity for team table
    /// </summary>
    [MetadataType(typeof(TeamMetadata))]
    public partial class Team
    {

    }

    /// <summary>
    /// Metadata class with validation annotations for team entity
    /// </summary>
    public class TeamMetadata
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot have more than 50 characters.")]
        public string name { get; set; }
    }
}