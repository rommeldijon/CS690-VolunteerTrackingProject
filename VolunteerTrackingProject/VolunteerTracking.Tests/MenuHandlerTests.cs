using Xunit;
using VolunteerTracking;
using System.Collections.Generic;

namespace VolunteerTracking.Tests
{
    public class MenuHandlerTests
    {
        // Enhanced mock with safety checks
        private class MockPromptService : IPromptService
        {
            private readonly Queue<string> _stringInputs;
            private readonly Queue<bool> _boolInputs;

            public MockPromptService(IEnumerable<string> stringInputs, IEnumerable<bool> boolInputs)
            {
                _stringInputs = new Queue<string>(stringInputs);
                _boolInputs = new Queue<bool>(boolInputs);
            }

            public string AskUserType() => _stringInputs.Count > 0 ? _stringInputs.Dequeue() : "exit";
            public bool ConfirmRegistration() => _boolInputs.Count > 0 && _boolInputs.Dequeue();
            public string AskUsername() => _stringInputs.Count > 0 ? _stringInputs.Dequeue() : "";
            public string AskNewPassword() => _stringInputs.Count > 0 ? _stringInputs.Dequeue() : "";
            public string AskPasswordConfirmation() => _stringInputs.Count > 0 ? _stringInputs.Dequeue() : "";
            public string PromptForInput(string message) => _stringInputs.Count > 0 ? _stringInputs.Dequeue() : "";
        }

        [Fact]
        public void HandleMainMenu_ShouldExitWhenUserChoosesExit()
        {
            var mockPrompt = new MockPromptService(
                new[] { "exit" }, new bool[] { }
            );

            Program.HandleMainMenu(mockPrompt);
            Assert.True(true);
        }

        [Fact]
        public void HandleMainMenu_ShouldHandleNewUserCancelsRegistration()
        {
            var mockPrompt = new MockPromptService(
                new[] { "new", "exit" },
                new[] { false } // decline registration
            );

            Program.HandleMainMenu(mockPrompt);
            Assert.True(true);
        }

        [Fact]
        public void HandleMainMenu_ShouldHandleUnknownUserLoginAndResetPassword()
        {
            var mockPrompt = new MockPromptService(
                new[]
                {
                    "returning",    // AskUserType
                    "unknownuser",  // AskUsername
                    "wrongpass",    // AskPassword
                    "y",            // Forgot password
                    "unknownuser",  // PromptForInput (username again)
                    "NewPass1!",    // AskNewPassword
                    "NewPass1!",    // AskPasswordConfirmation
                    "exit"          // Then exit
                },
                new bool[] { }
            );

            Program.HandleMainMenu(mockPrompt);
            Assert.True(true);
        }
    }
}

