using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;
using System.Diagnostics;

namespace RmsysCBT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject the Database Context
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // FIX: Fetch the tests from the DB so 'Model' is not null in the View
            // This prevents the NullReferenceException
            var tests = await _context.Tests.ToListAsync();
            return View(tests);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}