namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
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

}
