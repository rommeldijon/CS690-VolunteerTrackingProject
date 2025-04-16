namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
    public static Volunteer Authenticate(string username, string password, out bool userExists)
    {
        userExists = false;

        if (!File.Exists(filePath)) return null;

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var v = Volunteer.FromCsv(line);
            if (v.Username.ToLower() == username)
            {
                userExists = true;

                string hashed = Volunteer.HashPassword(password);
                if (v.Password == hashed)
                    return v;
            }
        }

        return null;
    }

    public static void ResetPassword(IPromptService prompt)
    {
        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine("[red] No users found.[/]");
            return;
        }

        var lines = File.ReadAllLines(filePath);

        while (true)
        {
            string username = prompt.AskUsername()?.Trim().ToLower();

            if (username == "exit")
            {
                AnsiConsole.MarkupLine("[gray](Returning to the main menu...)[/]");
                Thread.Sleep(1000);
                return;
            }

            bool found = false;

            for (int i = 0; i < lines.Length; i++)
            {
                var v = Volunteer.FromCsv(lines[i]);
                if (v.Username.ToLower() == username)
                {
                    AnsiConsole.MarkupLine("[blue] User verified. Let's reset your password.[/]");
                    string newPassword;

                    while (true)
                    {
                        string pass1 = prompt.AskNewPassword();
                        string pass2 = prompt.AskPasswordConfirmation();

                        if (pass1 != pass2)
                        {
                            AnsiConsole.MarkupLine("[red] Passwords do not match. Try again.[/]");
                            continue;
                        }

                        if (!Validator.IsPasswordValid(pass1))
                        {
                            AnsiConsole.MarkupLine("[red] Password does not meet requirements. Try again.[/]");
                            continue;
                        }

                        newPassword = Volunteer.HashPassword(pass1);
                        break;
                    }

                    v.Password = newPassword;
                    lines[i] = v.ToString();
                    File.WriteAllLines(filePath, lines);
                    AnsiConsole.MarkupLine("[green] Your new password has been set successfully![/]");
                    Thread.Sleep(1500);
                    return;
                }
            }

            // If no match was found
            AnsiConsole.MarkupLine("[red] Username not found. Please try again or type 'exit' to cancel.[/]");
        }
    }

}