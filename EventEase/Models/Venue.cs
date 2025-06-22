using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace EventEase.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Location { get; set; }

        [Range(1, 100)]
        public string Capacity { get; set; }

        [DisplayName("Upload Image")]
        public string? FileDetails { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        [Display(Name = "Image")]
        public string? ImageURL { get; set; }

        // Navigation property
        public List<Booking>? Bookings { get; set; }
    }
}
