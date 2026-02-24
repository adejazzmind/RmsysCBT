using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RmsysCBT.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. View all available exams
        public IActionResult Index()
        {
            var tests = _context.Tests.ToList();
            return View(tests);
        }

        // 2. View student's past exam history
        public IActionResult History()
        {
            var userName = User.Identity?.Name;

            // Uses StudentName to match your model perfectly
            var history = _context.TestResults
                                  .Where(tr => tr.StudentName == userName)
                                  .ToList();
            return View(history);
        }

        // 3. Take a specific exam
        public IActionResult Take(Guid id)
        {
            var test = _context.Tests
                               .Include(t => t.Questions)
                               .ThenInclude(q => q.Options)
                               .FirstOrDefault(t => t.Id == id);

            if (test == null) return NotFound();

            ViewBag.TestTitle = test.Title;
            ViewBag.Duration = test.DurationMinutes;
            ViewBag.TestId = test.Id;

            return View(test.Questions);
        }

        // 4. Grade the exam and save to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(Guid testId, Dictionary<Guid, Guid> answers)
        {
            var test = _context.Tests
                               .Include(t => t.Questions)
                               .FirstOrDefault(t => t.Id == testId);

            if (test == null) return NotFound();

            int score = 0;
            int totalQuestions = test.Questions.Count;

            if (answers != null)
            {
                foreach (var question in test.Questions)
                {
                    if (answers.TryGetValue(question.Id, out Guid selectedOptionId))
                    {
                        if (question.CorrectOptionId == selectedOptionId)
                        {
                            score++;
                        }
                    }
                }
            }

            // Create the result perfectly matching your AppModels.cs properties
            var testResult = new TestResult
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                StudentName = User.Identity?.Name ?? "Guest Student",
                Score = score,
                TotalQuestions = totalQuestions,
                TakenOn = DateTime.UtcNow
            };

            _context.TestResults.Add(testResult);
            _context.SaveChanges();

            return RedirectToAction("Result", new { id = testResult.Id });
        }

        // 5. Show the Result immediately after taking the exam
        public IActionResult Result(Guid id)
        {
            var userName = User.Identity?.Name;

            var result = _context.TestResults
                                 .FirstOrDefault(tr => tr.Id == id && tr.StudentName == userName);

            if (result == null) return NotFound();

            return View(result);
        }

        // 6. Show the Elite Certificate
        public IActionResult Certificate(Guid id)
        {
            var userName = User.Identity?.Name;

            var result = _context.TestResults
                                 .FirstOrDefault(tr => tr.Id == id && tr.StudentName == userName);

            if (result == null)
            {
                return NotFound();
            }

            // We deleted the Percentage check here! Let everyone see the certificate for testing.
            return View(result);
        }
    }
}