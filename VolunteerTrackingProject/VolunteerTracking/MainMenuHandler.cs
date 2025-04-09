namespace VolunteerTracking;

using System;
using VolunteerTracking.Models;

public static class MainMenuHandler
{
    public static void Run()
    {
        IPromptService promptService = new SpectrePromptService();
        Program.HandleMainMenu(promptService); // delegate logic to a testable method
    }
}
