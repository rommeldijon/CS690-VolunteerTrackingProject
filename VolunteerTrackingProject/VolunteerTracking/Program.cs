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
             MainMenuHandler.Run(); // now externalized and testable
        }
    }
}
