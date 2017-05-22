using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DentistBooking.Data;
using DentistBooking.Models;
using Microsoft.AspNetCore.Authorization;

namespace DentistBooking.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;    
        }


        // GET: Reservations
        [Authorize(Roles = "Admin")]
        public IActionResult Index(string sortOrder, string searchString)
        {
            var resultList = _context.Reservations
                .Include(r => r.Medic)
                .Include(r => r.Procedure)
                .OrderBy(r => r.FullName)
                .AsEnumerable();
            ViewData["SearchString"] = searchString;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParam"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["PhoneSortParam"] = sortOrder == "Phone" ? "phone_desc" : "Phone";

            if (!string.IsNullOrEmpty(searchString))
            {
                resultList = resultList.Where((r) => r.Medic.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    resultList = resultList.OrderByDescending((r) => r.FullName);
                    break;
                case "Date":
                    resultList = resultList.OrderBy(s => s.ReservationTime);
                    break;
                case "date_desc":
                    resultList = resultList.OrderByDescending(s => s.ReservationTime);
                    break;
                case "Phone":
                    resultList = resultList.OrderBy(s => s.Phone);
                    break;
                case "phone_desc":
                    resultList = resultList.OrderByDescending(s => s.Phone);
                    break;
            }
            return View(resultList.ToList());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Medic)
                .Include(r => r.Procedure)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["MedicId"] = new SelectList(_context.Medics, "Id", "Name");
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "Id", "Name");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Phone,Email,ReservationTime,Comments,MedicId,ProcedureId")] Reservation reservation)
        {
            bool evaluation = ModelState.IsValid;

            if (reservation.ReservationTime.CompareTo(DateTime.Now) > 0)
            {
                evaluation &= false;
                ModelState.AddModelError("", "You can not book in the past");
            }
            var oldReservation = _context.Reservations.FirstOrDefault((x) => x.Procedure.Id == reservation.ProcedureId
                && x.Medic.Id == reservation.MedicId
                && x.ReservationTime.CompareTo(reservation.ReservationTime) == 0);

            if (oldReservation != null)
            {
                evaluation &= false;
                ModelState.AddModelError("", "The current reservation is already booked! Please select another time");
            }

            if (evaluation)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["MedicId"] = new SelectList(_context.Medics, "Id", "Name", reservation.MedicId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "Id", "Name", reservation.ProcedureId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.SingleOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["MedicId"] = new SelectList(_context.Medics, "Id", "Id", reservation.MedicId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "Id", "Id", reservation.ProcedureId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Phone,Email,ReservationTime,Comments,MedicId,ProcedureId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["MedicId"] = new SelectList(_context.Medics, "Id", "Id", reservation.MedicId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "Id", "Id", reservation.ProcedureId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Medic)
                .Include(r => r.Procedure)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.SingleOrDefaultAsync(m => m.Id == id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
