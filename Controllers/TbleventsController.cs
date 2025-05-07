using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;

namespace EquidCMS.Controllers
{
    public class TbleventsController : Controller
    {
        private readonly EquiDbContext _context;

        public TbleventsController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: Tblevents
        public async Task<IActionResult> Index()
        {
            var equiDbContext = _context.Tblevents.Include(t => t.Event).Include(t => t.EventPricingType).Include(t => t.EventType).Include(t => t.Theme);
            return View(await equiDbContext.ToListAsync());
        }

        // GET: Tblevents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblevent = await _context.Tblevents
                .Include(t => t.Event)
                .Include(t => t.EventPricingType)
                .Include(t => t.EventType)
                .Include(t => t.Theme)
                .FirstOrDefaultAsync(m => m.Eventid == id);
            if (tblevent == null)
            {
                return NotFound();
            }

            return View(tblevent);
        }

        // GET: Tblevents/Create
        public IActionResult Create()
        {
            ViewData["Eventid"] = new SelectList(_context.Tblevents, "Eventid", "Eventid");
            ViewData["EventPricingTypeId"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtypeid");
            ViewData["EventTypeId"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtypeid");
            ViewData["Themeid"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId");
            return View();
        }

        // POST: Tblevents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Eventid,Themeid,Numberofattended,Descriptionofevent,Startdateofevent,Enddateofevent,Isvalidate,Createdby,Createdon,Updatedby,Updatedon,Evidenceid,EventTypeId,EventTime,EventVenue,EventLink,EventRegistrationLink,EventSpeaker,EventSpeakerOrg,EventHost,EventHostOrg,EventName,EventPricingTypeId,EventPricing,EventAgendaDoc,Parking,WheelChair")] Tblevent tblevent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblevent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Eventid"] = new SelectList(_context.Tblevents, "Eventid", "Eventid", tblevent.Eventid);
            ViewData["EventPricingTypeId"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtypeid", tblevent.EventPricingTypeId);
            ViewData["EventTypeId"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtypeid", tblevent.EventTypeId);
            ViewData["Themeid"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblevent.Themeid);
            return View(tblevent);
        }

        // GET: Tblevents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblevent = await _context.Tblevents.FindAsync(id);
            if (tblevent == null)
            {
                return NotFound();
            }
            ViewData["Eventid"] = new SelectList(_context.Tblevents, "Eventid", "Eventid", tblevent.Eventid);
            ViewData["EventPricingTypeId"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtypeid", tblevent.EventPricingTypeId);
            ViewData["EventTypeId"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtypeid", tblevent.EventTypeId);
            ViewData["Themeid"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblevent.Themeid);
            return View(tblevent);
        }

        // POST: Tblevents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Eventid,Themeid,Numberofattended,Descriptionofevent,Startdateofevent,Enddateofevent,Isvalidate,Createdby,Createdon,Updatedby,Updatedon,Evidenceid,EventTypeId,EventTime,EventVenue,EventLink,EventRegistrationLink,EventSpeaker,EventSpeakerOrg,EventHost,EventHostOrg,EventName,EventPricingTypeId,EventPricing,EventAgendaDoc,Parking,WheelChair")] Tblevent tblevent)
        {
            if (id != tblevent.Eventid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblevent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbleventExists(tblevent.Eventid))
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
            ViewData["Eventid"] = new SelectList(_context.Tblevents, "Eventid", "Eventid", tblevent.Eventid);
            ViewData["EventPricingTypeId"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtypeid", tblevent.EventPricingTypeId);
            ViewData["EventTypeId"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtypeid", tblevent.EventTypeId);
            ViewData["Themeid"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblevent.Themeid);
            return View(tblevent);
        }

        // GET: Tblevents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblevent = await _context.Tblevents
                .Include(t => t.Event)
                .Include(t => t.EventPricingType)
                .Include(t => t.EventType)
                .Include(t => t.Theme)
                .FirstOrDefaultAsync(m => m.Eventid == id);
            if (tblevent == null)
            {
                return NotFound();
            }

            return View(tblevent);
        }

        // POST: Tblevents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblevent = await _context.Tblevents.FindAsync(id);
            if (tblevent != null)
            {
                _context.Tblevents.Remove(tblevent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbleventExists(int id)
        {
            return _context.Tblevents.Any(e => e.Eventid == id);
        }
    }
}
