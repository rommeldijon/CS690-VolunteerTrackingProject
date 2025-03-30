namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;


public partial class Program
{
    static string GetInputWithExit(string prompt)
    {
        Console.Write(prompt);
        string input = Console.ReadLine();

        if (input?.ToLower() == "exit")
            throw new OperationCanceledException("User exited to main menu.");

        return input;
    }

    static string NormalizeTime(string input)
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

    static string GetValidatedDate(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt);
            if (DateTime.TryParseExact(input, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out _))
                return input;

            AnsiConsole.MarkupLine("[red]Invalid date format. Please use mm/dd/yyyy.[/]");
        }
    }

    static string GetValidatedTime(string prompt)
    {
        while (true)
        {
            string input = GetInputWithExit(prompt).Replace(":", "").Trim();

            if (input == "exit")
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

    static List<Activity> SortActivitiesChronologically(List<Activity> activities)
    {
        return activities
            .OrderBy(a =>
            {
                DateTime.TryParse($"{a.Date} {a.StartTime}", out DateTime dt);
                return dt;
            })
            .ToList();
    }
}
