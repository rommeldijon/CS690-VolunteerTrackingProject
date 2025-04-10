namespace VolunteerTracking;

using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
    public static void HandleMainMenu(IPromptService promptService)
{
    while (true)
    {
        string userType = promptService.AskUserType();

        if (userType == "exit")
        {
            AnsiConsole.MarkupLine("[gray]Goodbye! Exiting the system...[/]");
            Environment.Exit(0);
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
                Console.Write("Enter your username (or type 'exit' to cancel): ");
                usernameInput = Console.ReadLine()?.Trim().ToLower();

                if (usernameInput == "exit")
                {
                    AnsiConsole.MarkupLine("[gray]Returning to the welcome screen...[/]");
                    Thread.Sleep(1000);
                    return;
                }

                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine("[red]⚠ No users found. Try registering first.[/]");
                    Thread.Sleep(1000);
                    return;
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
                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                if (volunteer.Password == Volunteer.HashPassword(password))
                {
                    ShowLoggedInMenu(volunteer);
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Error. Invalid password.[/]");
                    Console.Write("Forgot password? (y/n): ");
                    var resetChoice = Console.ReadLine();

                    if (resetChoice?.Trim().ToLower() == "y")
                    {
                        Console.Clear();
                        AnsiConsole.MarkupLine("[green]No worries. Let's set a new password.[/]");
                        ResetPassword(promptService);
                        break; // return to main menu after reset
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
