namespace VolunteerTracking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using VolunteerTracking.Models;
    using Spectre.Console;

    public partial class Program
    {
        static string filePath = "volunteers.txt";

        static void Main(string[] args)
        {
            while (true)
            {
                string userType = DisplayWelcomeMenu();

                if (userType == "exit")
                {
                    AnsiConsole.MarkupLine("[gray]Goodbye! Exiting the system...[/]");
                    return;
                }

                if (userType == "new")
                {
                    if (ConfirmRegistration())
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
                Console.Write("Enter username: ");
                string username = Console.ReadLine()?.ToLower();

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                var volunteer = Authenticate(username, password);

                if (volunteer != null)
                {
                    ShowLoggedInMenu(volunteer);
                }
                else
                {
                    Console.WriteLine("Login failed. Invalid username or password.");
                    Console.Write("Forgot password? (y/n): ");
                    var resetChoice = Console.ReadLine();
                    if (resetChoice?.ToLower() == "y")
                    {
                        Console.WriteLine("No worries. Let's set a new password.");
                        ResetPassword();
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
}             

