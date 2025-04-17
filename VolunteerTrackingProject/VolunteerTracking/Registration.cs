namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VolunteerTracking.Models;
using Spectre.Console;
using VolunteerTracking;

public partial class Program
{
    public static void Register()
    {
        Utils.ClearWithHeader("New Volunteer Registration");
        AnsiConsole.MarkupLine("[gray](Type 'exit', then press enter at anytime to cancel registration and return to main menu.)[/]\n");

        string validatedUsername;
        while (true)
        {
            try
            {
                // Show this before every prompt attempt
                AnsiConsole.MarkupLine("[gray]Username must be at least 6 characters, letters/numbers/spaces only.[/]");

                validatedUsername = Utils.GetInputWithExit("Enter desired username: ").ToLower();

                if (validatedUsername.Length < 6 || !validatedUsername.All(c => char.IsLetterOrDigit(c) || c == ' '))
                {
                    AnsiConsole.MarkupLine("[red]Invalid username. Try again.[/]");
                    continue;
                }

                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    if (lines.Any(line => Volunteer.FromCsv(line).Username.ToLower() == validatedUsername))
                    {
                        AnsiConsole.MarkupLine("[red]That username is already taken. Try again.[/]");
                        continue;
                    }
                }

                break; // passed all checks
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        string fullName;
        while (true)
        {
            try
            {
                fullName = Utils.GetInputWithExit("Enter your first and last name: ");
                var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                    break;

                AnsiConsole.MarkupLine("[red]Please enter exactly a first and last name (e.g., John Smith).[/]");
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        string password = "";
        while (true)
        {
            try
            {
                AnsiConsole.MarkupLine("[gray]Password must be at least 6 characters, include 1 capital letter and 1 special character (!@#$%^&*()).[/]");
                string pass1 = Utils.GetInputWithExit("Set a password: ");
                string pass2 = Utils.GetInputWithExit("Confirm password: ");

                if (pass1 != pass2)
                {
                    AnsiConsole.MarkupLine("[red]Passwords do not match. Try again.[/]");
                    continue;
                }

                if (!Validator.IsPasswordValid(pass1))
                {
                    AnsiConsole.MarkupLine("[red]Password does not meet requirements. Try again.[/]");
                    continue;
                }

                password = Volunteer.HashPassword(pass1);
                break;
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[gray]Registration cancelled.[/]");
                return;
            }
        }

        var newVolunteer = new Volunteer
        {
            Username = validatedUsername,
            FullName = fullName,
            Password = password
        };

        AnsiConsole.MarkupLine("\n[bold green] Registration complete! You can now log in.[/]");
        AnsiConsole.MarkupLine("[gray](Press Enter to return to the main menu)[/]");
        Console.ReadLine(); // gives user time to read before screen clears
    }
}
