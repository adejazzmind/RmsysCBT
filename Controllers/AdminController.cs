using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;

namespace RmsysCBT.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admins can see this
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Dashboard: List all results
        public async Task<IActionResult> Index()
        {
            var results = await _context.TestResults
                .OrderByDescending(r => r.TakenOn)
                .ToListAsync();

            return View(results);
        }
    }
}