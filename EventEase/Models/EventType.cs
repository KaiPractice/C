using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Display(Name = "Event Type")]
        [Required(ErrorMessage = "Event Type Name is required.")]
        public string? EventTypeName { get; set; }

        public List<Event>? Events { get; set; }
    }
}
