namespace VolunteerTracking
{
    using System;
    using VolunteerTracking.Models;
    using Spectre.Console;

    public partial class Program
    {
        static string filePath = "volunteers.txt";

        static void Main(string[] args)
        {
            var promptService = new SpectrePromptService();

            while (true)
            {
                HandleMainMenu(promptService);  // returns if "exit" or user cancels login
                // Optional: wait briefly or clear screen
                Thread.Sleep(500);
            }
        }
        private static string PromptForTimeWithAmPm(string label)
        {
            while (true)
            {
                string timeInput = Utils.GetInputWithExit($"{label} Time (e.g. 9, 930, or 10:00): ");
                timeInput = Utils.NormalizeTime(timeInput);

                string ampm = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose [yellow]AM or PM[/]")
                        .AddChoices("AM", "PM"));

                if (DateTime.TryParseExact($"{timeInput} {ampm}", new[] { "h:mm tt", "hh:mm tt" }, null, System.Globalization.DateTimeStyles.None, out _))
                {
                    string finalTime = $"{timeInput} {ampm}";
                    AnsiConsole.MarkupLine($"[bold yellow] {label} Time:[/] [green]{finalTime}[/]");
                    return finalTime;
                }

                Console.WriteLine(" Invalid time format. Please enter a time like 9:00, 10:30, or 1130.");
            }
        }
    }
}