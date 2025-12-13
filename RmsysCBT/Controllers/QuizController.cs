using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSYSCBT.Data;
using RMSYSCBT.Models.Entities;

namespace RMSYSCBT.Controllers;

public class QuizController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuizController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Quiz
    public async Task<IActionResult> Index()
    {
        var questions = await _context.Questions
            .AsNoTracking()
            .ToListAsync();

        return View(questions);
    }

    // GET: /Quiz/Take/{id}
    public async Task<IActionResult> Take(Guid id)
    {
        var question = await _context.Questions
            .Include(q => q.Options)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound();

        return View(question);
    }

    // POST: /Quiz/Submit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid questionId, Guid selectedOptionId)
    {
        var question = await _context.Questions
            .Include(q => q.Options)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
            return NotFound();

        var selectedOption = question.Options
            .FirstOrDefault(o => o.Id == selectedOptionId);

        if (selectedOption == null)
            return BadRequest("Invalid option selected.");

        bool isCorrect = selectedOption.Id == question.CorrectOptionId;

        ViewBag.Question = question.Text;
        ViewBag.SelectedOption = selectedOption.Text;
        ViewBag.IsCorrect = isCorrect;
        ViewBag.CorrectAnswer = question.Options
            .First(o => o.Id == question.CorrectOptionId)
            .Text;

        return View("Result");
    }
}
