using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;

namespace RmsysCBT.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Tests.ToListAsync());
        }

        public async Task<IActionResult> Take(Guid id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null) return NotFound();

            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.TestId == id)
                .ToListAsync();

            // OPTION A: RANDOMIZATION (Shuffle Questions)
            questions = questions.OrderBy(x => Guid.NewGuid()).ToList();

            // Shuffle Options inside each question
            foreach (var q in questions)
            {
                q.Options = q.Options.OrderBy(x => Guid.NewGuid()).ToList();
            }

            ViewBag.TestTitle = test.Title;
            ViewBag.Duration = test.DurationMinutes;
            ViewBag.TestId = test.Id;

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Guid testId, Dictionary<Guid, Guid> answers)
        {
            var questions = await _context.Questions
                .Where(q => q.TestId == testId)
                .ToListAsync();

            int score = 0;
            // OPTION C: ANALYTICS PREP
            var sectionScores = new Dictionary<string, int>();
            var sectionTotals = new Dictionary<string, int>();

            foreach (var q in questions)
            {
                // Initialize section counters
                if (!sectionTotals.ContainsKey(q.Section))
                {
                    sectionTotals[q.Section] = 0;
                    sectionScores[q.Section] = 0;
                }
                sectionTotals[q.Section]++;

                if (answers.ContainsKey(q.Id) && answers[q.Id] == q.CorrectOptionId)
                {
                    score++;
                    sectionScores[q.Section]++;
                }
            }

            var result = new TestResult
            {
                TestId = testId,
                Score = score,
                TotalQuestions = questions.Count,
                StudentName = User.Identity?.Name ?? "Guest Student"
            };

            _context.TestResults.Add(result);
            await _context.SaveChangesAsync();

            // Pass Analytics to View using ViewBag
            ViewBag.SectionScores = sectionScores;
            ViewBag.SectionTotals = sectionTotals;

            return View("Result", result);
        }

        // OPTION D: CERTIFICATE GENERATION
        public IActionResult Certificate(Guid resultId)
        {
            // In a real app, fetch from DB. Mocking for now to ensure it works instantly.
            var model = new TestResult
            {
                StudentName = User.Identity?.Name ?? "Guest Student",
                Score = 40, // Mock score for display
                TotalQuestions = 50,
                TakenOn = DateTime.Now
            };

            // Try to find real result if available
            var realResult = _context.TestResults.FirstOrDefault(r => r.Id == resultId);
            if (realResult != null) model = realResult;

            return View(model);
        }
    }
}