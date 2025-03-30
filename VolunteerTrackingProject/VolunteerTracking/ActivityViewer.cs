namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
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
}
