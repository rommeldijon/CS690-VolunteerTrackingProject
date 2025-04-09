namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;
using VolunteerTracking;

public partial class Program
{
    static void GenerateImpactReport(Volunteer volunteer)
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[bold underline green]Generate Impact Report[/]");
        Console.WriteLine("(Type 'exit' anytime then click 'enter' to return to the MAIN menu)\n");

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
                string input = Utils.GetInputWithExit("Enter start date (mm/dd/yyyy): ");
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
                string input = Utils.GetInputWithExit("Enter end date (mm/dd/yyyy): ");
                if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate))
                    break;

                AnsiConsole.MarkupLine("[red]Invalid date. Use format mm/dd/yyyy (e.g., 04/01/2025).[/]");
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        string orgFilter;
        try
        {
            orgFilter = Utils.GetInputWithExit("Filter by organization (leave blank to skip): ").Trim().ToLower();
        }
        catch (OperationCanceledException)
        {
            return;
        }

        string typeFilter;
        try
        {
            typeFilter = Utils.GetInputWithExit("Filter by activity type (leave blank to skip): ").Trim().ToLower();
        }
        catch (OperationCanceledException)
        {
            return;
        }

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

        activities = Utils.SortActivitiesChronologically(activities);

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

}
