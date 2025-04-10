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
                    a.StartTime = $"{Utils.GetValidatedTime("Enter new start time (e.g. 9 or 930): ")} {Utils.SelectAmOrPm()}";
                    break;

                case "End Time":
                    a.EndTime = $"{Utils.GetValidatedTime("Enter new end time (e.g. 1 or 130): ")} {Utils.SelectAmOrPm()}";
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



