namespace VolunteerTracking;

using System;
using Spectre.Console;

public partial class Program
{
    public static string DisplayWelcomeMenu()
    {
        Console.Clear();
        Console.WriteLine("\n === 📅 Welcome to the Volunteer Tracking System ===");

        string userType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Are you a new, returning user, or would you like to exit?[/]")
                .AddChoices(new[] { "New", "Returning", "Exit" })
        ).ToLower();

        return userType;
    }

    public static bool ConfirmRegistration()
    {
        string regChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Would you like to register as a new user?[/]")
                .AddChoices("Yes", "No")
        );

        return regChoice == "Yes";
    }
}
