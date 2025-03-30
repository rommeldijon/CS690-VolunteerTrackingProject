namespace VolunteerTracking;

using System;
using System.IO;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
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
                    Environment.Exit(0); // Immediately ends the program
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
}
