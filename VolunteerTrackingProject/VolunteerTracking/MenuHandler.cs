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
                return;
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

            // Returning User Login Flow
            string username = promptService.AskUsername()?.ToLower();
            
            string password = promptService.AskNewPassword();

            var volunteer = Authenticate(username, password);

            if (volunteer != null)
            {
                ShowLoggedInMenu(volunteer);
            }
            else
            {
                Console.WriteLine("Login failed. Invalid username or password.");
                Console.Write("Forgot password? (y/n): ");
                var resetChoice = promptService.AskPasswordConfirmation(); // reuse this for input

                if (resetChoice?.ToLower() == "y")
                {
                    Console.WriteLine("No worries. Let's set a new password.");
                    ResetPassword(promptService);
                }
                else
                {
                    Console.WriteLine("Okay. Returning to the welcome screen...");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
