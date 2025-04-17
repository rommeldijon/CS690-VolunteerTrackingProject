namespace VolunteerTracking;

using System;
using VolunteerTracking.Models;

public static class ActivityEditor
{
    public static void Edit(ref Activity a, IPromptService prompt)
    {
        Console.Clear();
        Console.WriteLine("=== Edit Activity ===");

        string field = prompt.PromptForInput("Which field would you like to edit?\n(Date, Start Time, End Time, Organization, Location, Activity Type, Note, Cancel):")
            .Trim();

        if (field.Equals("Cancel", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Edit canceled.");
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
                case "End Time":
                    while (true)
                    {
                        string newStart = (field == "Start Time")
                            ? $"{Utils.GetValidatedTime("Enter new start time (e.g. 9 or 930): ")} {Utils.SelectAmOrPm()}"
                            : a.StartTime;

                        string newEnd = (field == "End Time")
                            ? $"{Utils.GetValidatedTime("Enter new end time (e.g. 1 or 130): ")} {Utils.SelectAmOrPm()}"
                            : a.EndTime;

                        if (!Utils.IsStartBeforeEnd(newStart, newEnd))
                        {
                            Console.WriteLine(" [red]Start time must be earlier than end time.[/]");
                            continue;
                        }

                        if (field == "Start Time") a.StartTime = newStart;
                        if (field == "End Time") a.EndTime = newEnd;
                        break;
                    }
                    break;

                case "Organization":
                    a.Organization = prompt.PromptForInput("Enter new organization: ");
                    break;

                case "Location":
                    a.Location = prompt.PromptForInput("Enter new location: ");
                    break;

                case "Activity Type":
                    a.Type = prompt.PromptForInput("Enter new activity type: ");
                    break;

                case "Note":
                    a.Note = prompt.PromptForInput("Enter new note (or leave blank): ");
                    break;

                default:
                    Console.WriteLine("Invalid field name.");
                    break;
            }

            Console.WriteLine("Update successful!");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Edit canceled by user.");
        }
        prompt.WaitForUserAcknowledgement("[gray](Press Enter to return to the menu)[/]");
    }
}
