using Microsoft.AspNetCore.Identity;
using RmsysCBT.Models;

namespace RmsysCBT.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // 1. Create Roles
            string[] roles = { "Admin", "Student" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Create Default Admin (Password: Admin123)
            var adminEmail = "admin@rmsys.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await userManager.CreateAsync(admin, "Admin123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // 3. Create Default Student (Password: Student123)
            var studentEmail = "student@rmsys.com";
            if (await userManager.FindByEmailAsync(studentEmail) == null)
            {
                var student = new IdentityUser { UserName = studentEmail, Email = studentEmail, EmailConfirmed = true };
                await userManager.CreateAsync(student, "Student123");
                await userManager.AddToRoleAsync(student, "Student");
            }

            // 4. Seed Questions (Only if empty)
            if (context.Tests.Any()) return;

            var test = new Test
            {
                Title = "RmsysCBT Entry Assessment",
                DurationMinutes = 60,
                Description = "A professional assessment covering Basics, Software, Logic, and AI."
            };
            context.Tests.Add(test);
            context.SaveChanges();

            // 50 Questions Array
            var rawQuestions = new[]
            {
                // SECTION A: BASICS
                new { Sec = "Computer Basics", Q = "What is a computer?", Ans = "B", Opts = new[] { "A storage device", "A machine that processes data", "A typewriter", "A calculator" } },
                new { Sec = "Computer Basics", Q = "Which is NOT a basic component?", Ans = "D", Opts = new[] { "CPU", "Memory", "Monitor", "Printer" } },
                new { Sec = "Computer Basics", Q = "The CPU is mainly responsible for", Ans = "C", Opts = new[] { "Displaying", "Storing", "Processing", "Printing" } },
                new { Sec = "Computer Basics", Q = "Component that temporarily stores data?", Ans = "B", Opts = new[] { "Hard Disk", "RAM", "Flash Drive", "ROM" } },
                new { Sec = "Computer Basics", Q = "The correct order of data processing is", Ans = "C", Opts = new[] { "Out-In-Proc", "In-Out-Proc", "Input-Process-Output", "Proc-In-Out" } },

                // SECTION B: SOFTWARE
                new { Sec = "Software", Q = "Software refers to", Ans = "B", Opts = new[] { "Hardware", "Programs & Instructions", "Accessories", "Cables" } },
                new { Sec = "Software", Q = "Example of System Software?", Ans = "C", Opts = new[] { "Word", "Chrome", "Windows OS", "Photoshop" } },
                new { Sec = "Software", Q = "Example of Application Software?", Ans = "C", Opts = new[] { "BIOS", "Linux", "Excel", "Firmware" } },
                new { Sec = "Software", Q = "Programming software is used to", Ans = "B", Opts = new[] { "Play games", "Write programs", "Browse internet", "Print" } },
                new { Sec = "Software", Q = "Difference between hardware and software?", Ans = "A", Opts = new[] { "Hardware is visible, software is not", "Software is touchable", "Hardware runs on software", "Software is physical" } },

                // SECTION C: BINARY SYSTEMS
                new { Sec = "Binary Systems", Q = "Binary system uses base?", Ans = "D", Opts = new[] { "5", "8", "10", "2" } },
                new { Sec = "Binary Systems", Q = "Binary digits are?", Ans = "B", Opts = new[] { "1 & 2", "0 & 1", "0-9", "A-F" } },
                new { Sec = "Binary Systems", Q = "Why computers use binary?", Ans = "B", Opts = new[] { "Simple to understand", "Electronic circuits have 2 states", "Faster for humans", "Less memory" } },
                new { Sec = "Binary Systems", Q = "A bit refers to", Ans = "C", Opts = new[] { "4 binary digits", "8 binary digits", "Single binary digit", "A character" } },
                new { Sec = "Binary Systems", Q = "Eight bits make up a", Ans = "B", Opts = new[] { "Nibble", "Byte", "Word", "Register" } },

                // SECTION D: ALGORITHMS
                new { Sec = "Algorithms", Q = "An algorithm is", Ans = "C", Opts = new[] { "Language", "Device", "Step-by-step instructions", "Tool" } },
                new { Sec = "Algorithms", Q = "Everyday algorithm example?", Ans = "B", Opts = new[] { "TV", "Recipe", "Sleeping", "Music" } },
                new { Sec = "Algorithms", Q = "Flowcharts are used to", Ans = "C", Opts = new[] { "Write code", "Store data", "Represent algorithms visually", "Design web" } },
                new { Sec = "Algorithms", Q = "Pseudocode is?", Ans = "C", Opts = new[] { "Language", "Machine code", "Plain language logic description", "Binary" } },
                new { Sec = "Algorithms", Q = "Logical AND returns TRUE when?", Ans = "B", Opts = new[] { "One true", "Both true", "Both false", "One false" } },

                // SECTION E: PROGRAMMING CONCEPTS
                new { Sec = "Programming", Q = "Programming language used to?", Ans = "B", Opts = new[] { "Repair", "Communicate with computers", "Design hardware", "Store" } },
                new { Sec = "Programming", Q = "High-level language example?", Ans = "C", Opts = new[] { "Machine Code", "Assembly", "Python", "Binary" } },
                new { Sec = "Programming", Q = "Variables are used to?", Ans = "B", Opts = new[] { "Control", "Store Data", "Display", "Print" } },
                new { Sec = "Programming", Q = "Decision making statement?", Ans = "C", Opts = new[] { "for", "while", "if-else", "function" } },
                new { Sec = "Programming", Q = "Loops are used to?", Ans = "B", Opts = new[] { "Stop", "Repeat actions", "Store", "Compare" } },

                 // SECTION F: WEB DEV
                new { Sec = "Web Dev", Q = "HTML is for?", Ans = "C", Opts = new[] { "Styling", "Interactivity", "Structure", "Hosting" } },
                new { Sec = "Web Dev", Q = "CSS is for?", Ans = "B", Opts = new[] { "Structure", "Styling", "Logic", "Data" } },
                new { Sec = "Web Dev", Q = "JavaScript is used to?", Ans = "C", Opts = new[] { "Style", "Store", "Add interactivity", "Database" } },
                new { Sec = "Web Dev", Q = "The internet works via?", Ans = "C", Opts = new[] { "Hardware only", "Software only", "Client-server communication", "Local storage" } },
                new { Sec = "Web Dev", Q = "Website accessed using?", Ans = "B", Opts = new[] { "Compiler", "Browser", "Editor", "Database" } },

                // SECTION G: DATA & SECURITY
                new { Sec = "Security & Data", Q = "An array is?", Ans = "B", Opts = new[] { "Single value", "Collection of elements", "Database", "Function" } },
                new { Sec = "Security & Data", Q = "Dictionary stores data as?", Ans = "C", Opts = new[] { "Numbers", "Values", "Key-value pairs", "Arrays" } },
                new { Sec = "Security & Data", Q = "Cybersecurity focuses on?", Ans = "B", Opts = new[] { "Speed", "Protecting Systems", "Writing Code", "Designing" } },
                new { Sec = "Security & Data", Q = "Encryption used to?", Ans = "B", Opts = new[] { "Delete", "Protect data", "Display", "Copy" } },
                new { Sec = "Security & Data", Q = "Firewalls help to?", Ans = "B", Opts = new[] { "Store", "Block unauthorized access", "Speed up", "Install" } },
                
                // SECTION H: AI & LOGIC
                new { Sec = "AI & Logic", Q = "Artificial Intelligence is?", Ans = "C", Opts = new[] { "Hardware", "Human intellect", "Machines mimicking humans", "Natural" } },
                new { Sec = "AI & Logic", Q = "Machine learning allows systems to?", Ans = "B", Opts = new[] { "Manual program", "Learn from data", "Store", "Shutdown" } },
                new { Sec = "AI & Logic", Q = "AI commonly used in?", Ans = "C", Opts = new[] { "Calculators", "Word", "Voice assistants", "Printers" } },
                new { Sec = "AI & Logic", Q = "Decomposition means?", Ans = "C", Opts = new[] { "Combine", "Ignore", "Break into smaller parts", "Repeat" } },
                new { Sec = "AI & Logic", Q = "Logical OR returns TRUE when?", Ans = "B", Opts = new[] { "Both false", "One or both true", "Both true only", "None" } }
            };

            foreach (var item in rawQuestions)
            {
                var question = new Question { Text = item.Q, Section = item.Sec, TestId = test.Id };
                context.Questions.Add(question);

                Option correctOption = null;
                var optionEntities = new List<Option>();

                for (int i = 0; i < item.Opts.Length; i++)
                {
                    var opt = new Option { Text = item.Opts[i], QuestionId = question.Id };
                    optionEntities.Add(opt);

                    int correctIndex = item.Ans[0] - 'A';
                    if (i == correctIndex) correctOption = opt;
                }

                context.Options.AddRange(optionEntities);
                if (correctOption != null) question.CorrectOptionId = correctOption.Id;
            }
            context.SaveChanges();
        }
    }
}