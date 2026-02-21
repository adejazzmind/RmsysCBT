using System;
using System.Collections.Generic;

namespace RmsysCBT.Models
{
    // --- DATABASE ENTITIES ---
    public class Test
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Title { get; set; }
        public string Description { get; set; } = "No description provided.";
        public int DurationMinutes { get; set; } = 30;
        public List<Question> Questions { get; set; } = new();
    }

    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Text { get; set; }
        public string Section { get; set; } = "General"; // NEW: For Analytics
        public Guid TestId { get; set; }
        public Test Test { get; set; } = null!;
        public List<Option> Options { get; set; } = new();
        public Guid CorrectOptionId { get; set; }
    }

    public class Option
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Text { get; set; }
        public Guid QuestionId { get; set; }
    }

    public class TestResult
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TestId { get; set; }
        public string StudentName { get; set; } = "Guest Student";
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime TakenOn { get; set; } = DateTime.UtcNow;

        public int Percentage => TotalQuestions == 0 ? 0 : (int)((double)Score / TotalQuestions * 100);
        public bool Passed => Percentage >= 50;
    }

    // --- VIEW MODELS ---
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class QuizResultViewModel
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
    }
}