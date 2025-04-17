using System;
using System.IO;
using System.Linq;
using VolunteerTracking.Models;
using Xunit;

namespace VolunteerTracking.Tests
{
    public class AuthenticationTests
    {
        private readonly string testFilePath = "volunteers_test.txt";

        public AuthenticationTests()
        {
            // Redirect filePath for testing to avoid modifying the real data
            typeof(Program).GetField("filePath", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
                .SetValue(null, testFilePath);
        }

        [Fact]
        public void ResetPassword_ShouldUpdatePassword_WhenInputsAreValid()
        {
            // Arrange
            string originalPassword = "OldPass!";
            string hashedPassword = Volunteer.HashPassword(originalPassword);
            var volunteer = new Volunteer
            {
                Username = "testuser",
                FullName = "Test User",
                Password = hashedPassword
            };

            File.WriteAllText(testFilePath, volunteer.ToString() + Environment.NewLine);

            var mockPrompt = new MockPromptService
            {
                Username = "testuser",
                NewPassword = "NewPass1!",
                ConfirmPassword = "NewPass1!"
            };

            // Act
            Program.ResetPassword(mockPrompt);

            // Assert
            var lines = File.ReadAllLines(testFilePath);
            Assert.Single(lines);
            var updatedVolunteer = Volunteer.FromCsv(lines[0]);
            Assert.NotEqual(hashedPassword, updatedVolunteer.Password);
            Assert.Equal(Volunteer.HashPassword("NewPass1!"), updatedVolunteer.Password);
        }

        ~AuthenticationTests()
        {
            // Cleanup
            if (File.Exists(testFilePath))
                File.Delete(testFilePath);
        }

        // Simple mock for IPromptService
        private class MockPromptService : IPromptService
        {
            public string Username { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }

            public string AskUsername() => Username;
            public string AskNewPassword() => NewPassword;
            public string AskPasswordConfirmation() => ConfirmPassword;
            public string PromptForInput(string message) => "";
            public string AskUserType() => "";
            public bool ConfirmRegistration() => false;
            public void WaitForUserAcknowledgement(string message)
            {
                // No-op
            }
        }
    }
}
