namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VolunteerTracking.Models;
using Spectre.Console;

public partial class Program
{
    static void Register()
    {
        AnsiConsole.MarkupLine("[gray](Type 'exit', then press enter at anytime to cancel registration and return to main menu.)[/]\n");

        string validatedUsername;
        while (true)
        {
            try
            {
                validatedUsername = GetInputWithExit("Enter desired username: ").ToLower();

                if (validatedUsername.Length < 6 || !validatedUsername.All(c => char.IsLetterOrDigit(c) || c == ' '))
                {
                    AnsiConsole.MarkupLine("[red]Username must be at least 6 characters and contain only letters, numbers, or spaces.[/]");
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

                break;
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
                fullName = GetInputWithExit("Enter your first and last name: ");
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
                string pass1 = GetInputWithExit("Set a password (min 6 chars, 1 capital letter, 1 special character): ");
                string pass2 = GetInputWithExit("Confirm password: ");

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

        File.AppendAllText(filePath, newVolunteer.ToString() + Environment.NewLine);
        AnsiConsole.MarkupLine("[green]Registration complete! You can now log in.[/]");
    }
}
