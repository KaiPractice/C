using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        [Display(Name = "Event Type")]
        public int? EventTypeID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Description { get; set; }

        // Navigation property
        public List<Booking>? Bookings { get; set; }

        // Navigation property other
        public EventType? EventType { get; set; }
    }
}
