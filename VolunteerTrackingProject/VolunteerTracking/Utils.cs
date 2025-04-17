namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using Spectre.Console;
using VolunteerTracking.Models;

public static class Utils
{
    public static string GetInputWithExit(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (input?.ToLower() == "exit")
            throw new OperationCanceledException("User exited to main menu.");

        return input;
    }

    public static string NormalizeTime(string input)
    {
        input = input.Trim();

        if (input.Length == 1 || input.Length == 2)
            return $"{int.Parse(input)}:00";

        if (input.Length == 3)
            return $"{int.Parse(input.Substring(0, 1))}:{input.Substring(1)}";

        if (input.Length == 4)
            return $"{int.Parse(input.Substring(0, 2))}:{input.Substring(2)}";

        return input; // assume already formatted
    }

    public static string GetValidatedDate(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt);
            if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                return input;

            AnsiConsole.MarkupLine("[red]Invalid date format. Please use mm/dd/yyyy.[/]");
        }
    }

    public static string GetValidatedTime(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt).Replace(":", "").Trim();

            if (input.ToLower() == "exit")
                throw new OperationCanceledException();

            if (int.TryParse(input, out int timeNum))
            {
                if (timeNum >= 1 && timeNum <= 12)
                    return $"{timeNum}:00";
                if (timeNum >= 100 && timeNum <= 1259)
                    return $"{timeNum / 100}:{(timeNum % 100):D2}";
            }

            AnsiConsole.MarkupLine("[red]Invalid time. Enter in 12-hour format like 9 or 930.[/]");
        }
    }

    public static List<Activity> SortActivitiesChronologically(List<Activity> activities)
    {
        return activities
            .OrderBy(a =>
            {
                DateTime.TryParse($"{a.Date} {a.StartTime}", out DateTime dt);
                return dt;
            })
            .ToList();
    }

    public static string SelectAmOrPm()
    {
        string[] options = { "AM", "PM" };
        int selectedIndex = 0;
        ConsoleKey key;

        int cursorTop = Console.CursorTop;

        Console.WriteLine("\nUse ← → arrow keys to select AM or PM. Press Enter to confirm:");

        do
        {
            Console.SetCursorPosition(0, cursorTop + 1);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, cursorTop + 1);

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

        Console.WriteLine();
        return options[selectedIndex];

    }
    public static bool IsStartBeforeEnd(string startTime, string endTime)
    {
        return DateTime.TryParse(startTime, out DateTime start) &&
            DateTime.TryParse(endTime, out DateTime end) &&
            start < end;
    }

    public static void ClearWithHeader(string title)
    {
        Console.Clear();
        AnsiConsole.MarkupLine($"[bold yellow]{title}[/]");
        Console.WriteLine();
    }
}
