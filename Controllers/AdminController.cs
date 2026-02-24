using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;
using System;
using System.Linq;

namespace RmsysCBT.Controllers
{
    // For now, any logged-in user can see this, but later we will lock it to just the "Admin" role!
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard: Lists all tests
        public IActionResult Index()
        {
            // We Include(t => t.Questions) so we can count how many questions are in each test
            var tests = _context.Tests.Include(t => t.Questions).ToList();
            return View(tests);
        } // <--- THIS WAS THE MISSING BRACE!

        // 1. Shows the empty form to create a new test
        public IActionResult Create()
        {
            return View();
        }

        // 2. Grabs the data from the form and saves it to SQLite
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Test test)
        {
            if (ModelState.IsValid)
            {
                test.Id = Guid.NewGuid(); // Give the new test a unique ID
                _context.Tests.Add(test);
                _context.SaveChanges(); // Save to database!

                return RedirectToAction(nameof(Index)); // Send admin back to the dashboard
            }
            return View(test); // If there's an error, show the form again
        }
    }
}