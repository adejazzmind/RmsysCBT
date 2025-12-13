using Microsoft.EntityFrameworkCore;
using RMSYSCBT.Data;
using RMSYSCBT.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// =======================
// SERVICES
// =======================
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// =======================
// BUILD APP
// =======================
var app = builder.Build();

// =======================
// DATABASE MIGRATION + SEED DATA
// =======================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    if (!context.Questions.Any())
    {
        var questions = new List<Question>();

        void AddQuestion(
            int number,
            string questionText,
            string correct,
            params string[] wrong)
        {
            var questionId = Guid.NewGuid();
            var correctOptionId = Guid.NewGuid();

            var options = new List<Option>
            {
                new Option
                {
                    Id = correctOptionId,
                    Text = correct,
                    QuestionId = questionId
                }
            };

            foreach (var w in wrong)
            {
                options.Add(new Option
                {
                    Id = Guid.NewGuid(),
                    Text = w,
                    QuestionId = questionId
                });
            }

            questions.Add(new Question
            {
                Id = questionId,
                Text = $"{number}. {questionText}",
                CorrectOptionId = correctOptionId,
                Options = options
            });
        }

        // =======================
        // 15 COMPUTER QUESTIONS
        // =======================

        AddQuestion(1, "What does CPU stand for?",
            "Central Processing Unit",
            "Central Process Unit",
            "Computer Personal Unit",
            "Central Performance Utility");

        AddQuestion(2, "What does RAM stand for?",
            "Random Access Memory",
            "Read Access Memory",
            "Rapid Action Memory",
            "Random Allocation Machine");

        AddQuestion(3, "Which device is used to type text?",
            "Keyboard",
            "Monitor",
            "Printer",
            "Speaker");

        AddQuestion(4, "Which of these is an operating system?",
            "Windows",
            "Microsoft Word",
            "Google Chrome",
            "Intel");

        AddQuestion(5, "What does HDD stand for?",
            "Hard Disk Drive",
            "High Data Disk",
            "Hard Digital Device",
            "Hybrid Data Drive");

        AddQuestion(6, "Which unit measures CPU speed?",
            "Hertz",
            "Bytes",
            "Pixels",
            "Watts");

        AddQuestion(7, "Which is NOT a programming language?",
            "HTML",
            "C#",
            "Python",
            "Java");

        AddQuestion(8, "What does GPU stand for?",
            "Graphics Processing Unit",
            "General Processing Unit",
            "Graphical Performance Utility",
            "Graphics Power Unit");

        AddQuestion(9, "Which memory is volatile?",
            "RAM",
            "ROM",
            "Hard Disk",
            "SSD");

        AddQuestion(10, "Which device displays output?",
            "Monitor",
            "Keyboard",
            "Mouse",
            "Scanner");

        AddQuestion(11, "Which of these is a web browser?",
            "Google Chrome",
            "Windows",
            "Linux",
            "Oracle");

        AddQuestion(12, "What does URL stand for?",
            "Uniform Resource Locator",
            "Universal Resource Link",
            "Uniform Reference Line",
            "Universal Retrieval Locator");

        AddQuestion(13, "Which memory is permanent?",
            "ROM",
            "RAM",
            "Cache",
            "Registers");

        AddQuestion(14, "What does SSD stand for?",
            "Solid State Drive",
            "Super Speed Disk",
            "System Storage Device",
            "Secondary Storage Disk");

        AddQuestion(15, "Which of these is an input device?",
            "Mouse",
            "Monitor",
            "Printer",
            "Speaker");

        context.Questions.AddRange(questions);
        context.SaveChanges();
    }
}

// =======================
// MIDDLEWARE PIPELINE
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// =======================
// ROUTING
// =======================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
