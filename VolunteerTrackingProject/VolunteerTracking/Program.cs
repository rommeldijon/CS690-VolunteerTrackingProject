namespace VolunteerTracking;

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

                continue; // Back to welcome menu
            }
            else
            {
                AnsiConsole.MarkupLine("[gray]Okay. Returning to the main menu...[/]");
                Thread.Sleep(1000);
                continue;
            }
        }

        // ✅ Returning User Login Flow
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


    static bool IsPasswordValid(string password)
    {
        if (password.Length < 6)
            return false;

        bool hasUpper = false;
        bool hasSpecial = false;
        string specialChars = "!@#$%^&*()";

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (specialChars.Contains(c)) hasSpecial = true;
        }

        return hasUpper && hasSpecial;
    }

    static bool IsUsernameValid(string username)
{
    if (string.IsNullOrWhiteSpace(username) || username.Length < 6)
        return false;

    foreach (char c in username)
    {
        if (!char.IsLetterOrDigit(c) && c != ' ')
            return false;
    }

    return true;
}


    static string SelectAmOrPm()
    {
        string[] options = { "AM", "PM" };
        int selectedIndex = 0;
        ConsoleKey key;

        int cursorTop = Console.CursorTop; // Remember where to draw

        Console.WriteLine("\nUse ← → arrow keys to select AM or PM. Press Enter to confirm:");

        do
        {
            // Move to fixed line for redrawing
            Console.SetCursorPosition(0, cursorTop + 1);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear line
            Console.SetCursorPosition(0, cursorTop + 1);         // Return to start

            // Draw options
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write($" {options[i]} ");
                Console.ResetColor();
                Console.Write("   ");
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.RightArrow && selectedIndex < options.Length - 1)
                selectedIndex++;
            else if (key == ConsoleKey.LeftArrow && selectedIndex > 0)
                selectedIndex--;

        } while (key != ConsoleKey.Enter);

        Console.WriteLine(); // Move down when done
        return options[selectedIndex];
    }
     
}

