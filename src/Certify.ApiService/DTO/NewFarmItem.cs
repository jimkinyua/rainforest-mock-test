using System.ComponentModel.DataAnnotations;

namespace Certify.ApiService.DTO
{
    public class NewFarmItem
    {
        [Required]
        [MinLength(6)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MinLength(4)]
        public string Country { get; set; } = string.Empty;
    }
}
