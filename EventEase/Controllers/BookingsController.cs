using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.ViewModels;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = from b in _context.Booking
                           .Include(b => b.Event)
                           .Include(b => b.Venue)
                           select b;

            return View(await bookings.ToListAsync());
        }

        public async Task<IActionResult> SearchBooking(SearchViewModel model)
        {
            var query = _context.Booking.Include(e => e.Event).ThenInclude(et => et.EventType).Include(e => e.Venue).AsQueryable();
            
            if (model.SearchBookingId.HasValue)
            {
                query = query.Where(e => e.BookingId >= model.SearchBookingId);
            }
            if (!string.IsNullOrEmpty(model.SearchEventName))
            {
                query = query.Where(e => e.Event.EventName.Contains(model.SearchEventName));
            }
            if (!string.IsNullOrEmpty(model.SearchEventTypeName))
            {
                query = query.Where(et => et.Event.EventType.EventTypeName.Contains(model.SearchEventTypeName));
            }
            // Check if StartDate and EndDate are provided
            if (model.StartDate.HasValue && model.EndDate.HasValue)
            {
                query = query.Where(e => e.StartDate >= model.StartDate.Value && e.EndDate <= model.EndDate.Value);
            }
            model.Bookings = await query.ToListAsync();
            // Populate EventTypes for the dropdown
            model.EventTypes = await _context.EventType.ToListAsync();
            return View(model);
        }



        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,StartDate,EndDate,VenueId,EventId")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                bool isOverlapping = await _context.Booking.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    ((booking.StartDate >= b.StartDate && booking.StartDate <= b.EndDate) ||
                     (booking.EndDate >= b.StartDate && booking.EndDate <= b.EndDate) ||
                     (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate)));

                if (isOverlapping)
                {
                    ModelState.AddModelError(string.Empty, "The selected date range overlaps with an existing booking for this venue.");
                    ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueId);
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueId);
            return View(booking);
        }


        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,StartDate,EndDate,VenueId,EventId")] Booking booking)
        {
            if (id != booking.BookingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                bool isOverlapping = await _context.Booking.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingId != booking.BookingId && // Exclude the current booking
                    ((booking.StartDate >= b.StartDate && booking.StartDate <= b.EndDate) ||
                     (booking.EndDate >= b.StartDate && booking.EndDate <= b.EndDate) ||
                     (booking.StartDate <= b.StartDate && booking.EndDate >= b.EndDate)));

                if (isOverlapping)
                {
                    ModelState.AddModelError(string.Empty, "The selected date range overlaps with an existing booking for this venue.");
                    ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
                    ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueId);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venue, "VenueID", "VenueName", booking.VenueId);
            return View(booking);
        }


        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.BookingId == id);
        }
    }
}
