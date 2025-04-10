// ActivityEditor.cs
namespace VolunteerTracking;

using System;
using Spectre.Console;
using VolunteerTracking.Models;

public static class ActivityEditor
{
    public static void Edit(ref Activity a)
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
                    a.Date = Utils.GetValidatedDate("Enter new date (mm/dd/yyyy): ");
                    break;

                case "Start Time":
                    a.StartTime = $"{Utils.GetValidatedTime("Enter new start time (e.g. 9 or 930): ")} {Utils.SelectAmOrPm()}";
                    break;

                case "End Time":
                    a.EndTime = $"{Utils.GetValidatedTime("Enter new end time (e.g. 1 or 130): ")} {Utils.SelectAmOrPm()}";
                    break;

                case "Organization":
                    a.Organization = Utils.GetInputWithExit("Enter new organization: ");
                    break;

                case "Location":
                    a.Location = Utils.GetInputWithExit("Enter new location: ");
                    break;

                case "Activity Type":
                    a.Type = Utils.GetInputWithExit("Enter new activity type: ");
                    break;

                case "Note":
                    a.Note = Utils.GetInputWithExit("Enter new note (or leave blank): ");
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
