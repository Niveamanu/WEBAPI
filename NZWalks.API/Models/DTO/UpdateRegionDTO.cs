using System.ComponentModel.DataAnnotations;

namespace NZWalks.API.Models.DTO
{
    public class UpdateRegionDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "Code can be Min of 3 characters")]
        [MaxLength(3, ErrorMessage = "Code can be Max of 3 characters")]
        public string Code { get; set; }

        [Required]
        [MaxLength(3, ErrorMessage = "name can be Max of 100 characters")]
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
