using Spectre.Console;

namespace VolunteerTracking;

public class SpectrePromptService : IPromptService
{
    public string AskUserType()
    {
        Console.Clear();
        Console.WriteLine("\n === Welcome to the Volunteer Tracking System ===");

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Are you a new, returning user, or would you like to exit?[/]")
                .AddChoices("New", "Returning", "Exit")
        ).ToLower();
    }

    public bool ConfirmRegistration()
    {
        string regChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Would you like to register as a new user?[/]")
                .AddChoices("Yes", "No")
        );

        return regChoice == "Yes";
    }

    public string AskUsername()
    {
        Console.Write("Enter your username: ");
        return Console.ReadLine();
    }

    public string AskNewPassword()
    {
        Console.Write("Enter password: ");
        return Console.ReadLine();
    }

    public string AskPasswordConfirmation()
    {
        Console.Write("Confirm password: ");
        return Console.ReadLine();
    }

    public string PromptForInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }
}
