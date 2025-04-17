namespace VolunteerTracking;

using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
    public static bool HandleMainMenu(IPromptService promptService)
    {
        while (true)
        {
            string userType = promptService.AskUserType();

            if (userType == "exit")
            {
                AnsiConsole.MarkupLine("[gray]Goodbye! Exiting the system...[/]");
                return true; //  user wants to exit
            }

            if (userType == "new")
            {
                if (promptService.ConfirmRegistration())
                {
                    try
                    {
                        Register();
                    }
                    catch (OperationCanceledException)
                    {
                        AnsiConsole.MarkupLine("[gray]Registration canceled. Returning to the main menu...[/]");
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                else
                {
                    AnsiConsole.MarkupLine("[gray]Okay. Returning to the main menu...[/]");
                    Thread.Sleep(1000);
                    continue;
                }
            }

            // === Returning User Login Flow ===
            Volunteer volunteer = null;
            string usernameInput = "";

            while (true)
            {
                usernameInput = promptService.PromptForInput("Enter your username (or type 'exit' to cancel):").Trim().ToLower();

                if (usernameInput == "exit")
                {
                    AnsiConsole.MarkupLine("[gray]Returning to the welcome screen...[/]");
                    Thread.Sleep(1000);
                    return false;
                }

                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine("[red]No users found. Try registering first.[/]");
                    Thread.Sleep(1000);
                    return false;
                }

                var allVolunteers = File.ReadAllLines(filePath)
                    .Select(Volunteer.FromCsv)
                    .ToList();

                volunteer = allVolunteers.FirstOrDefault(v => v.Username.ToLower() == usernameInput);

                if (volunteer == null)
                {
                    AnsiConsole.MarkupLine("[red]Username not found. Try again or type 'exit' to cancel.[/]");
                    continue;
                }

                // At this point, username is valid. Ask for password.
                string password = promptService.PromptForInput("Enter password:");

                if (volunteer.Password == Volunteer.HashPassword(password))
                {
                    ShowLoggedInMenu(volunteer);
                    return false; //  Login successful, continue app loop
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Error. Invalid password.[/]");
                    var resetChoice = promptService.PromptForInput("Forgot password? (y/n):").Trim().ToLower();

                    if (resetChoice == "y")
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[green]No worries. Let's set a new password.[/]");
                        ResetPassword(promptService);
                        return false; //  Return to main menu after reset
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[gray]Okay. Try again or type 'exit' next time to cancel.[/]");
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
