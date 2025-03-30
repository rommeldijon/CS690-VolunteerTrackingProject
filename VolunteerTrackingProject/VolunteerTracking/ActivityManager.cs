namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
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
}
