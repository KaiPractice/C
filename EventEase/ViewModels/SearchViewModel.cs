using EventEase.Models;

namespace EventEase.ViewModels
{
    public class SearchViewModel
    {
        public List<Booking>? Bookings { get; set; }
        public List<Event>? Events { get; set; }
        public List<EventType>? EventTypes { get; set; }
        public string? SearchEventName { get; set; }
        public int? SearchBookingId { get; set; }
        public int? SearchEventType { get; set; }
        // New properties for date range
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
