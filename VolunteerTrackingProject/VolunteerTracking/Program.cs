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
    }
}