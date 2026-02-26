using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;
using System;
using System.Linq;

namespace RmsysCBT.Controllers
{
    // We are locking this down to ONLY people with the "Admin" role!
    [Authorize(Roles = "Admin")]
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
        }

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

        // 3. View ALL Student Results
        public IActionResult Results()
        {
            // Get all results, ordered by newest first
            var allResults = _context.TestResults.OrderByDescending(r => r.TakenOn).ToList();
            return View(allResults);
        }

        // 4. Show the form to add a question to a specific test
        public IActionResult AddQuestion(Guid id)
        {
            var test = _context.Tests.FirstOrDefault(t => t.Id == id);
            if (test == null) return NotFound();

            ViewBag.TestTitle = test.Title;
            ViewBag.TestId = test.Id;
            return View();
        }

        // 5. Save the new question and its options to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddQuestion(Guid testId, string questionText, string section, string optionA, string optionB, string optionC, string optionD, string correctAnswer)
        {
            // 1. Create the Question
            var question = new Question
            {
                Id = Guid.NewGuid(),
                TestId = testId,
                Text = questionText,
                Section = string.IsNullOrWhiteSpace(section) ? "General" : section
            };
            _context.Questions.Add(question);

            // 2. Create the Options
            var optA = new Option { Id = Guid.NewGuid(), QuestionId = question.Id, Text = optionA };
            var optB = new Option { Id = Guid.NewGuid(), QuestionId = question.Id, Text = optionB };
            var optC = new Option { Id = Guid.NewGuid(), QuestionId = question.Id, Text = optionC };
            var optD = new Option { Id = Guid.NewGuid(), QuestionId = question.Id, Text = optionD };

            _context.Options.AddRange(optA, optB, optC, optD);

            // 3. Figure out which one is the correct answer
            if (correctAnswer == "A") question.CorrectOptionId = optA.Id;
            else if (correctAnswer == "B") question.CorrectOptionId = optB.Id;
            else if (correctAnswer == "C") question.CorrectOptionId = optC.Id;
            else if (correctAnswer == "D") question.CorrectOptionId = optD.Id;

            _context.SaveChanges();

            // Send them back to the dashboard
            return RedirectToAction(nameof(Index));
        }

        // 6. Securely delete a test (and all its questions)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteTest(Guid id)
        {
            // Find the test in the database
            var test = _context.Tests.FirstOrDefault(t => t.Id == id);

            if (test != null)
            {
                // Remove it and save the changes
                // Note: Entity Framework will automatically delete the linked questions too!
                _context.Tests.Remove(test);
                _context.SaveChanges();
            }

            // Refresh the dashboard
            return RedirectToAction(nameof(Index));
        }
    }
}