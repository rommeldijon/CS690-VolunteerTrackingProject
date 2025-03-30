namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using VolunteerTracking.Models;
using Spectre.Console;

class Program
{
    static string filePath = "volunteers.txt";

    static void Main(string[] args)
    {
        while (true) // Infinite loop until the user chooses to exit
        {
            Console.Clear();
            Console.WriteLine("\n === 📅 Welcome to the Volunteer Tracking System ===");

            string userType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Are you a new, returning user, or would you like to exit?[/]")
                    .AddChoices(new[] { "New", "Returning", "Exit" })
            ).ToLower();

            if (userType == "exit")
            {
                AnsiConsole.MarkupLine("[gray]Goodbye! Exiting the system...[/]");
                return;
            }

            if (userType == "new")
            {
                string regChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Would you like to register as a new user?[/]")
                        .AddChoices("Yes", "No")
                );

                if (regChoice == "Yes")
                {
                    try
                    {
                        Register(); // Call your Register() with built-in exit support
                    }
                    catch (OperationCanceledException)
                    {
                        AnsiConsole.MarkupLine("[gray]Registration canceled. Returning to the main menu...[/]");
                        Thread.Sleep(1000);
                    }

                    continue; // Always return to welcome screen after registration attempt
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]Okay. Returning to the main menu...[/]");
                    Thread.Sleep(1000);
                    continue;
                }
            }

            // === Returning User Login ===
            Console.Write("Enter username: ");
            string username = Console.ReadLine()?.ToLower();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var volunteer = Authenticate(username, password);

            if (volunteer != null)
            {
                ShowLoggedInMenu(volunteer);
            }
            else
            {
                Console.WriteLine("Login failed. Invalid username or password.");
                Console.Write("Forgot password? (y/n): ");
                var resetChoice = Console.ReadLine();
                if (resetChoice?.ToLower() == "y")
                {
                    Console.WriteLine("No worries. Let's set a new password.");
                    ResetPassword();
                }
                else
                {
                    Console.WriteLine("Okay. Returning to the welcome screen...");
                    Thread.Sleep(1000);
                }
            }

            // The loop restarts, sending user back to the welcome screen
        }
    }


    
    
    static Volunteer Authenticate(string username, string password)
    {
        if (!File.Exists(filePath))
            return null;

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var v = Volunteer.FromCsv(line);
            string hashed = Volunteer.HashPassword(password);
            if (v.Username.ToLower() == username && v.Password == hashed)
                return v;
        }

        return null;
    }



    static void Register()
    {
        AnsiConsole.MarkupLine("[gray](Type 'exit', then press enter at anytime to cancel registration and return to main menu.)[/]\n");

        string validatedUsername;
        while (true)
        {
            try
            {
                validatedUsername = GetInputWithExit("Enter desired username: ").ToLower();

                // Username validation
                if (validatedUsername.Length < 6 || !validatedUsername.All(c => char.IsLetterOrDigit(c) || c == ' '))
                {
                    AnsiConsole.MarkupLine("[red]Username must be at least 6 characters and contain only letters, numbers, or spaces.[/]");
                    continue;
                }

                // Check for duplicates
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    if (lines.Any(line => Volunteer.FromCsv(line).Username.ToLower() == validatedUsername))
                    {
                        AnsiConsole.MarkupLine("[red]That username is already taken. Try again.[/]");
                        continue;
                    }
                }

                break; // valid and available
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        string fullName;
        while (true)
        {
            try
            {
                fullName = GetInputWithExit("Enter your first and last name: ");
                var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                    break;

                AnsiConsole.MarkupLine("[red]Please enter exactly a first and last name (e.g., John Smith).[/]");
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        string password = "";
        while (true)
        {
            try
            {
                string pass1 = GetInputWithExit("Set a password (min 6 chars, 1 capital letter, 1 special character): ");
                string pass2 = GetInputWithExit("Confirm password: ");

                if (pass1 != pass2)
                {
                    AnsiConsole.MarkupLine("[red]Passwords do not match. Try again.[/]");
                    continue;
                }

                if (!IsPasswordValid(pass1))
                {
                    AnsiConsole.MarkupLine("[red]Password does not meet requirements. Try again.[/]");
                    continue;
                }

                password = Volunteer.HashPassword(pass1);
                break;
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        var newVolunteer = new Volunteer
        {
            Username = validatedUsername,
            FullName = fullName,
            Password = password
        };

        File.AppendAllText(filePath, newVolunteer.ToString() + Environment.NewLine);
        AnsiConsole.MarkupLine("[green]Registration complete! You can now log in.[/]");
    }



    static bool IsPasswordValid(string password)
    {
        if (password.Length < 6)
            return false;

        bool hasUpper = false;
        bool hasSpecial = false;
        string specialChars = "!@#$%^&*()";

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (specialChars.Contains(c)) hasSpecial = true;
        }

        return hasUpper && hasSpecial;
    }

    static bool IsUsernameValid(string username)
{
    if (string.IsNullOrWhiteSpace(username) || username.Length < 6)
        return false;

    foreach (char c in username)
    {
        if (!char.IsLetterOrDigit(c) && c != ' ')
            return false;
    }

    return true;
}


   static void ResetPassword()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine()?.ToLower();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("No users found.");
            return;
        }

        var lines = File.ReadAllLines(filePath);
        bool found = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var v = Volunteer.FromCsv(lines[i]);
            if (v.Username.ToLower() == username)
            {
                Console.WriteLine("User verified. Let's reset your password.");

                string newPassword = "";
                while (true)
                {
                    Console.Write("New password (min 6 chars, 1 capital letter, 1 special character {!@#$%^&*()}): ");
                    string pass1 = Console.ReadLine();

                    Console.Write("Confirm new password: ");
                    string pass2 = Console.ReadLine();

                    if (pass1 != pass2)
                    {
                        Console.WriteLine("Passwords do not match. Try again.");
                        continue;
                    }

                    if (!IsPasswordValid(pass1))
                    {
                        Console.WriteLine("Password does not meet requirements. Try again.");
                        continue;
                    }

                    newPassword = Volunteer.HashPassword(pass1);
                    break;
                }

                v.Password = newPassword;
                lines[i] = v.ToString();
                found = true;
                break;
            }
        }

        if (found)
        {
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Password successfully reset.");
        }
        else
        {
            Console.WriteLine("User not found or information does not match.");
        }
    }


    static void ShowLoggedInMenu(Volunteer volunteer)
{
    while (true)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold green]Welcome, {volunteer.FullName}![/]");
        AnsiConsole.MarkupLine("[gray](Use arrow keys to navigate, then press Enter)[/]");

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[bold yellow]What would you like to do?[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Log a New Activity",
                    "View Upcoming Activities",
                    "Edit/Cancel an Activity",
                    "Generate an Impact Report",
                    "Log-Off"
                }));

        Console.Clear();

        switch (choice)
        {
            case "Log a New Activity":
                LogNewActivity(volunteer);
                break;

            case "View Upcoming Activities":
                ViewUpcomingActivities(volunteer);
                break;

            case "Edit/Cancel an Activity":
                ManageActivities(volunteer);
                break;

            case "Generate an Impact Report":
                GenerateImpactReport(volunteer);
                break;

            case "Log-Off":
            var confirmLogout = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Are you sure you want to log off?[/]")
                    .AddChoices(new[] { "Yes", "No" }));

                if (confirmLogout == "Yes")
                {
                    AnsiConsole.MarkupLine("[red]Logging off. Goodbye![/]");
                    return;
                }
                else
                {
                    AnsiConsole.MarkupLine("[green]Logout canceled. Returning to menu...[/]");
                }
                break;

        }

        AnsiConsole.MarkupLine("\n[gray](Press Enter to return to the menu)[/]");
        Console.ReadLine();
    }
}


    static string SelectAmOrPm()
    {
        string[] options = { "AM", "PM" };
        int selectedIndex = 0;
        ConsoleKey key;

        int cursorTop = Console.CursorTop; // Remember where to draw

        Console.WriteLine("\nUse ← → arrow keys to select AM or PM. Press Enter to confirm:");

        do
        {
            // Move to fixed line for redrawing
            Console.SetCursorPosition(0, cursorTop + 1);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear line
            Console.SetCursorPosition(0, cursorTop + 1);         // Return to start

            // Draw options
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write($" {options[i]} ");
                Console.ResetColor();
                Console.Write("   ");
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow && selectedIndex < options.Length - 1)
                selectedIndex++;
            else if (key == ConsoleKey.LeftArrow && selectedIndex > 0)
                selectedIndex--;

        } while (key != ConsoleKey.Enter);

        Console.WriteLine(); // Move down when done
        return options[selectedIndex];
    }


    static void LogNewActivity(Volunteer volunteer)
    {
        Console.Clear();
        Console.WriteLine("=== Log a New Activity ===");
        Console.WriteLine("(Type 'exit' to cancel anytime)");
        try
        {
            string org = GetInputWithExit("Enter the Organization Name: ");
            string address = GetInputWithExit("Address: ");

            string sameLocation = GetInputWithExit("Is the activity location the same as the organization address? (y/n): ").ToLower();
            string location = address;

            if (sameLocation != "y")
            {
                location = GetInputWithExit("Enter Activity Location: ");
            }

            string date;
            while (true)
            {
                date = GetInputWithExit("Enter the Date (mm/dd/yyyy): ");
                if (DateTime.TryParseExact(date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                    break;

                Console.WriteLine("⚠ Invalid date format. Please enter as mm/dd/yyyy (e.g. 04/01/2025).");
            }

            Console.WriteLine("\n⚠ Please enter time in 12-hour format (e.g. 9:00, 10:30).");
            Console.WriteLine("   You can also enter shortcuts like '9', '930', or '1130', and it will auto-correct.\n");

            string startTime;
            while (true)
            {
                string timeInput = GetInputWithExit("Start Time (e.g. 9, 930, or 10:00): ");
                timeInput = NormalizeTime(timeInput);

                string ampm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose [yellow]AM or PM[/]")
                        .AddChoices("AM", "PM"));

                if (DateTime.TryParseExact($"{timeInput} {ampm}", new[] { "h:mm tt", "hh:mm tt" }, null, System.Globalization.DateTimeStyles.None, out _))
                {
                    startTime = $"{timeInput} {ampm}";
                    break;
                }

                Console.WriteLine("⚠ Invalid time format. Please enter a time like 9:00, 10:30, or 1130.");
            }

            string endTime;
            while (true)
            {
                string timeInput = GetInputWithExit("End Time (e.g. 1, 130, or 1:30): ");
                timeInput = NormalizeTime(timeInput);

                string ampm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose [yellow]AM or PM[/]")
                        .AddChoices("AM", "PM"));

                if (DateTime.TryParseExact($"{timeInput} {ampm}", new[] { "h:mm tt", "hh:mm tt" }, null, System.Globalization.DateTimeStyles.None, out _))
                {
                    endTime = $"{timeInput} {ampm}";
                    break;
                }

                Console.WriteLine("⚠ Invalid time format. Please enter a time like 1:00, 2:30, or 1130.");
            }

            string type = GetInputWithExit("Enter the Activity Type: ");
            string note = GetInputWithExit("Optional: Add a note about this activity: ");

            var activity = new Activity
            {
                Username = volunteer.Username,
                Organization = org,
                Address = address,
                Location = location,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Type = type,
                Note = note
            };

            // === Conflict Check ===
            if (File.Exists("activities.txt"))
            {
                var allActivities = File.ReadAllLines("activities.txt")
                    .Select(line => Activity.FromCsv(line))
                    .Where(a => a.Username.ToLower() == volunteer.Username.ToLower() && a.Date == date)
                    .ToList();

                DateTime.TryParse($"{date} {startTime}", out DateTime newStart);
                DateTime.TryParse($"{date} {endTime}", out DateTime newEnd);

                foreach (var existing in allActivities)
                {
                    DateTime.TryParse($"{existing.Date} {existing.StartTime}", out DateTime existingStart);
                    DateTime.TryParse($"{existing.Date} {existing.EndTime}", out DateTime existingEnd);

                    bool overlap = newStart < existingEnd && newEnd > existingStart;
                    if (overlap)
                    {
                        Console.WriteLine("\n⚠ WARNING: This activity conflicts with another one:");
                        Console.WriteLine($"- Existing: {existing.StartTime}–{existing.EndTime} at {existing.Organization}");
                        Console.Write("Do you still want to save this activity? (y/n): ");
                        string proceed = Console.ReadLine()?.ToLower();
                        if (proceed != "y")
                        {
                            Console.WriteLine("Activity was not saved due to conflict.");
                            return;
                        }
                        break;
                    }
                }
            }

            File.AppendAllText("activities.txt", activity.ToString() + Environment.NewLine);
            Console.WriteLine("\n✅ Activity logged successfully!");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nLogging activity canceled. Returning to main menu.");
        }
    }


    static void ViewUpcomingActivities(Volunteer volunteer)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold yellow]=== Your Upcoming Activities ===[/]");

        if (!File.Exists("activities.txt"))
        {
            AnsiConsole.MarkupLine("[red]No activities found.[/]");
            return;
        }

        var allLines = File.ReadAllLines("activities.txt").ToList();
        var activities = new List<Activity>();
        var indexes = new List<int>();

        for (int i = 0; i < allLines.Count; i++)
        {
            var a = Activity.FromCsv(allLines[i]);
            if (a.Username.ToLower() == volunteer.Username.ToLower())
            {
                activities.Add(a);
                indexes.Add(i);
            }
        }

        if (activities.Count == 0)
        {
            AnsiConsole.MarkupLine("[gray]You have no upcoming activities.[/]");
            return;
        }

        activities = SortActivitiesChronologically(activities);

        // Create a  table
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .Title("[bold green]Upcoming Activities[/]")
            .AddColumn(new TableColumn("No.").Centered())
            .AddColumn(new TableColumn("Date").Centered())
            .AddColumn(new TableColumn("Time").Centered())
            .AddColumn(new TableColumn("Organization").Width(20).NoWrap())
            .AddColumn(new TableColumn("Location").Width(20).NoWrap())
            .AddColumn(new TableColumn("Activity Type").Width(15).NoWrap())
            .AddColumn(new TableColumn("Note").Width(25).NoWrap());


        bool hasConflict = false;

            for (int i = 0; i < activities.Count; i++)
            {
                var a = activities[i];
                bool isConflict = false;

                DateTime.TryParse($"{a.Date} {a.StartTime}", out DateTime startA);
                DateTime.TryParse($"{a.Date} {a.EndTime}", out DateTime endA);

                for (int j = 0; j < activities.Count; j++)
                {
                    if (i == j) continue;
                    var b = activities[j];

                    if (b.Date == a.Date)
                    {
                        DateTime.TryParse($"{b.Date} {b.StartTime}", out DateTime startB);
                        DateTime.TryParse($"{b.Date} {b.EndTime}", out DateTime endB);

                        if (startA < endB && endA > startB)
                        {
                            isConflict = true;
                            hasConflict = true;
                            break;
                        }
                    }
                }

                string wrap(string text) => isConflict ? $"[red]{text}[/]" : text;

                table.AddRow(
                    wrap((i + 1).ToString()),
                    wrap(a.Date),
                    isConflict
                        ? $"[red]{a.StartTime} - {a.EndTime} ⚠ Conflict[/]"
                        : $"{a.StartTime} - {a.EndTime}",
                    wrap(a.Organization),
                    wrap(a.Location),
                    wrap(a.Type),
                    wrap(string.IsNullOrWhiteSpace(a.Note) ? "-" : a.Note)
                );
            }


        AnsiConsole.Write(table);
         
        if (hasConflict)
        {
            AnsiConsole.MarkupLine("[red]⚠ Some activities have conflicting times. Please review those marked in red.[/]");
        }


        var activityChoices = new List<string>();
        for (int i = 0; i < activities.Count; i++)
        {
            var a = activities[i];
            activityChoices.Add($"{i + 1}. {a.Date} | {a.StartTime}-{a.EndTime} | {a.Organization}");
        }
        activityChoices.Add("Return to menu");

        string selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[bold]Select an activity to manage:[/]")
                .PageSize(10)
                .AddChoices(activityChoices));

        if (selected == "Return to menu")
            return;

        int selectedIndex = activityChoices.IndexOf(selected);
        var selectedActivity = activities[selectedIndex];
        int fileIndex = indexes[selectedIndex];

        Console.Clear();
        AnsiConsole.MarkupLine("[bold cyan]Selected Activity Details:[/]");
        AnsiConsole.MarkupLine($"[bold]Date:[/] {selectedActivity.Date}");
        AnsiConsole.MarkupLine($"[bold]Time:[/] {selectedActivity.StartTime} - {selectedActivity.EndTime}");
        AnsiConsole.MarkupLine($"[bold]Organization:[/] {selectedActivity.Organization}");
        AnsiConsole.MarkupLine($"[bold]Location:[/] {selectedActivity.Location}");
        AnsiConsole.MarkupLine($"[bold]Activity Type:[/] {selectedActivity.Type}");
        AnsiConsole.MarkupLine($"[bold]Note:[/] {selectedActivity.Note}");

        string action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[green]What would you like to do?[/]")
                .AddChoices(new[] {
                    "Mark as Completed",
                    "Edit this Activity",
                    "Cancel/Delete this Activity",
                    "Return to menu"
                }));

        switch (action)
        {
            case "Mark as Completed":
                File.AppendAllText("completed_activities.txt", fileIndex.ToString() + Environment.NewLine);
                AnsiConsole.MarkupLine("[green]Marked as completed.[/]");
                break;

            case "Edit this Activity":
                EditActivity(ref selectedActivity);
                allLines[fileIndex] = selectedActivity.ToString();
                File.WriteAllLines("activities.txt", allLines);
                AnsiConsole.MarkupLine("[green]Activity updated.[/]");
                break;

            case "Cancel/Delete this Activity":
                allLines.RemoveAt(fileIndex);
                File.WriteAllLines("activities.txt", allLines);
                AnsiConsole.MarkupLine("[red]Activity deleted.[/]");
                break;

            default:
                return;
        }

        AnsiConsole.MarkupLine("\n[gray](Press Enter to return to the menu)[/]");
        Console.ReadLine();
    }


    static void EditActivity(ref Activity a)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold yellow]=== Edit Activity ===[/]");

        var field = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Which field would you like to edit?")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Date", "Start Time", "End Time",
                    "Organization", "Location",
                    "Activity Type", "Note", "Cancel Edit"
                }));

        if (field == "Cancel Edit")
        {
            AnsiConsole.MarkupLine("[gray]Edit canceled.[/]");
            return;
        }

        try
        {
            switch (field)
            {
                case "Date":
                    string newDate = GetValidatedDate("Enter new date (mm/dd/yyyy): ");
                    a.Date = newDate;
                    break;

                case "Start Time":
                    string newStartTime = GetValidatedTime("Enter new start time (e.g. 9 or 930): ");
                    string startAmPm = SelectAmOrPm();
                    a.StartTime = $"{newStartTime} {startAmPm}";
                    break;

                case "End Time":
                    string newEndTime = GetValidatedTime("Enter new end time (e.g. 1 or 130): ");
                    string endAmPm = SelectAmOrPm();
                    a.EndTime = $"{newEndTime} {endAmPm}";
                    break;

                case "Organization":
                    a.Organization = GetInputWithExit("Enter new organization: ");
                    break;

                case "Location":
                    a.Location = GetInputWithExit("Enter new location: ");
                    break;

                case "Activity Type":
                    a.Type = GetInputWithExit("Enter new activity type: ");
                    break;

                case "Note":
                    a.Note = GetInputWithExit("Enter new note (or leave blank): ");
                    break;
            }

            AnsiConsole.MarkupLine("[green]Update successful![/]");
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[gray]Edit canceled by user.[/]");
        }

        AnsiConsole.MarkupLine("\n[gray](Press Enter to return to the menu)[/]");
        Console.ReadLine();
    }

    
    static void ManageActivities(Volunteer volunteer)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold underline green]Edit or Cancel an Activity[/]");

            if (!File.Exists("activities.txt"))
            {
                AnsiConsole.MarkupLine("[red]No activities found.[/]");
                return;
            }

            var allLines = File.ReadAllLines("activities.txt").ToList();
            var activities = new List<Activity>();
            var indexes = new List<int>();

            for (int i = 0; i < allLines.Count; i++)
            {
                var a = Activity.FromCsv(allLines[i]);
                if (a.Username.ToLower() == volunteer.Username.ToLower())
                {
                    activities.Add(a);
                    indexes.Add(i);
                }
            }

            if (activities.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]You have no activities.[/]");
                return;
            }

            // Sort chronologically
            activities = SortActivitiesChronologically(activities);

            // === Display Table ===
            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title("[bold green]Your Activities[/]")
                .AddColumn("Date")
                .AddColumn("Time")
                .AddColumn("Organization")
                .AddColumn("Location")
                .AddColumn("Activity Type")
                .AddColumn("Note");

            foreach (var a in activities)
            {
                table.AddRow(
                    a.Date,
                    $"{a.StartTime} - {a.EndTime}",
                    a.Organization,
                    a.Location,
                    a.Type,
                    a.Note ?? ""
                );
            }

            AnsiConsole.Write(table);

            // === Selection Menu ===
            var activityChoices = activities
                .Select((a, i) => $"{i + 1}. {a.Date} | {a.StartTime}-{a.EndTime} | {a.Organization}")
                .ToList();
            activityChoices.Add("Return to Main Menu");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an activity to manage:[/]")
                    .PageSize(10)
                    .AddChoices(activityChoices)
            );

            if (selection == "Return to Main Menu")
                return;

            int selectedIndex = int.Parse(selection.Split('.')[0]) - 1;
            var selectedActivity = activities[selectedIndex];
            int originalIndex = indexes[selectedIndex];

            // === Action Choice ===
            Console.Clear();
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]What would you like to do with this activity?[/]")
                    .AddChoices(new[] { "Edit this Activity", "Cancel/Delete this Activity", "Return to menu" }));

            switch (action)
            {
                case "Edit this Activity":
                    EditActivity(ref selectedActivity);
                    allLines[originalIndex] = selectedActivity.ToString();
                    File.WriteAllLines("activities.txt", allLines);
                    AnsiConsole.MarkupLine("[green]Activity updated.[/]");
                    break;

                case "Cancel/Delete this Activity":
                    allLines.RemoveAt(originalIndex);
                    File.WriteAllLines("activities.txt", allLines);
                    AnsiConsole.MarkupLine("[red]Activity deleted.[/]");
                    break;

                default:
                    return;
            }

            AnsiConsole.MarkupLine("\n[gray](Press Enter to return to the menu)[/]");
            Console.ReadLine();
        }


    static void GenerateImpactReport(Volunteer volunteer)
{
    Console.Clear();
    AnsiConsole.MarkupLine("[bold underline green]Generate Impact Report[/]");
    Console.WriteLine("(Type 'exit'anytime then click 'enter' to return to the MAIN menu)\n");

    if (!File.Exists("activities.txt"))
    {
        AnsiConsole.MarkupLine("[red]No activities found.[/]");
        return;
    }

    // === Prompt Filters ===
    DateTime startDate, endDate;

    while (true)
    {
        try
        {
            string input = GetInputWithExit("Enter start date (mm/dd/yyyy): ");
            if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate))
                break;
            AnsiConsole.MarkupLine("[red]Invalid date. Use format mm/dd/yyyy (e.g., 04/01/2025).[/]");
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    while (true)
    {
        try
        {
            string input = GetInputWithExit("Enter end date (mm/dd/yyyy): ");
            if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate))
                break;
            AnsiConsole.MarkupLine("[red]Invalid date. Use format mm/dd/yyyy (e.g., 04/01/2025).[/]");
        }
        catch (OperationCanceledException)
        {
            return;
        }
    }

    string orgFilter = AnsiConsole.Ask<string>("Filter by [blue]organization[/] (leave blank to skip):").Trim().ToLower();
    string typeFilter = AnsiConsole.Ask<string>("Filter by [blue]activity type[/] (leave blank to skip):").Trim().ToLower();
    bool includeNotes = AnsiConsole.Confirm("Include notes in the report?");

    var activities = File.ReadAllLines("activities.txt")
        .Select(line => Activity.FromCsv(line))
        .Where(a => a.Username.ToLower() == volunteer.Username.ToLower())
        .Where(a =>
        {
            DateTime.TryParse(a.Date, out DateTime date);
            return date >= startDate && date <= endDate;
        })
        .Where(a => string.IsNullOrEmpty(orgFilter) || a.Organization.ToLower().Contains(orgFilter))
        .Where(a => string.IsNullOrEmpty(typeFilter) || a.Type.ToLower().Contains(typeFilter))
        .ToList();

    activities = SortActivitiesChronologically(activities);

    Console.Clear();
    AnsiConsole.MarkupLine("[bold green]=== Impact Report ===[/]");
    AnsiConsole.MarkupLine($"User: [blue]{volunteer.FullName}[/]");
    AnsiConsole.MarkupLine($"Date Range: [cyan]{startDate:MM/dd/yyyy}[/] - [cyan]{endDate:MM/dd/yyyy}[/]");
    if (!string.IsNullOrWhiteSpace(orgFilter))
        AnsiConsole.MarkupLine($"Filtered by Organization: [italic]{orgFilter}[/]");
    if (!string.IsNullOrWhiteSpace(typeFilter))
        AnsiConsole.MarkupLine($"Filtered by Activity Type: [italic]{typeFilter}[/]");

    AnsiConsole.MarkupLine($"Total Matching Activities: [bold yellow]{activities.Count}[/]");

    var table = new Table()
        .RoundedBorder()
        .BorderColor(Color.Grey)
        .Title("[bold yellow]Impact Report Details[/]")
        .AddColumn("Date")
        .AddColumn("Time")
        .AddColumn("Organization")
        .AddColumn("Location")
        .AddColumn("Type");

    if (includeNotes)
        table.AddColumn("Note");

    foreach (var a in activities)
    {
        var row = new List<string> { a.Date, $"{a.StartTime} - {a.EndTime}", a.Organization, a.Location, a.Type };
        if (includeNotes) row.Add(a.Note ?? "");
        table.AddRow(row.ToArray());
    }

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine("\n[gray](Press Enter to return to the menu)[/]");
    Console.ReadLine();
}
    


        static List<Activity> SortActivitiesChronologically(List<Activity> activities)
    {
        return activities
            .OrderBy(a =>
            {
                DateTime.TryParse($"{a.Date} {a.StartTime}", out DateTime dt);
                return dt;
            })
            .ToList();
    }


    static string GetInputWithExit(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (input?.ToLower() == "exit")
            throw new OperationCanceledException("User exited to main menu.");

        return input;
    }


    static string NormalizeTime(string input)
    {
        input = input.Trim();

        if (input.Length == 1 || input.Length == 2)
            return $"{int.Parse(input)}:00";

        if (input.Length == 3)
            return $"{int.Parse(input.Substring(0, 1))}:{input.Substring(1)}";

        if (input.Length == 4)
            return $"{int.Parse(input.Substring(0, 2))}:{input.Substring(2)}";

        return input; // assume already formatted
    }
    static string GetValidatedDate(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt);
            if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                return input;

            AnsiConsole.MarkupLine("[red]Invalid date format. Please use mm/dd/yyyy.[/]");
        }
    }

    static string GetValidatedTime(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt).Replace(":", "").Trim();

            if (input == "exit")
                throw new OperationCanceledException();

            if (int.TryParse(input, out int timeNum))
            {
                if (timeNum >= 1 && timeNum <= 12)
                    return $"{timeNum}:00";
                if (timeNum >= 100 && timeNum <= 1259)
                    return $"{timeNum / 100}:{(timeNum % 100):D2}";
            }

            AnsiConsole.MarkupLine("[red]Invalid time. Enter in 12-hour format like 9 or 930.[/]");
        }
    }
}

