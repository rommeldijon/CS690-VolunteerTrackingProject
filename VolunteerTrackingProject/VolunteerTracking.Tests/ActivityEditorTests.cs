using Xunit;
using VolunteerTracking;
using VolunteerTracking.Models;
using System.Collections.Generic;
using System;

namespace VolunteerTracking.Tests
{
    public class ActivityEditorTests
    {
        // ✅ Updated MockPromptService with all required methods
        private class MockPromptService : IPromptService
        {
            private readonly Queue<string> _responses;

            public MockPromptService(IEnumerable<string> responses)
            {
                _responses = new Queue<string>(responses);
            }

            public string PromptForInput(string message)
            {
                return _responses.Count > 0 ? _responses.Dequeue() : "";
            }

            public void WaitForUserAcknowledgement(string message)
            {
                // No-op for tests (prevents Console.ReadLine())
            }

            public string AskUserType() => throw new NotImplementedException();
            public bool ConfirmRegistration() => throw new NotImplementedException();
            public string AskUsername() => throw new NotImplementedException();
            public string AskNewPassword() => throw new NotImplementedException();
            public string AskPasswordConfirmation() => throw new NotImplementedException();
        }

        [Fact]
        public void Edit_ShouldUpdateOrganization()
        {
            // Arrange
            var activity = new Activity { Organization = "Old Org" };
            var mockPrompt = new MockPromptService(new[] { "Organization", "New Org" });

            // Act
            ActivityEditor.Edit(ref activity, mockPrompt);

            // Assert
            Assert.Equal("New Org", activity.Organization);
        }

        [Fact]
        public void Edit_ShouldUpdateNote()
        {
            var activity = new Activity { Note = "Old note" };
            var mockPrompt = new MockPromptService(new[] { "Note", "Updated note" });

            ActivityEditor.Edit(ref activity, mockPrompt);

            Assert.Equal("Updated note", activity.Note);
        }

        [Fact]
        public void Edit_ShouldNotChangeAnything_WhenCanceled()
        {
            var activity = new Activity { Location = "123 Main St" };
            var mockPrompt = new MockPromptService(new[] { "Cancel" });

            ActivityEditor.Edit(ref activity, mockPrompt);

            Assert.Equal("123 Main St", activity.Location);
        }

        [Fact]
        public void Edit_ShouldHandleInvalidFieldGracefully()
        {
            var activity = new Activity { Type = "Cleanup" };
            var mockPrompt = new MockPromptService(new[] { "InvalidField" });

            ActivityEditor.Edit(ref activity, mockPrompt);

            Assert.Equal("Cleanup", activity.Type); // no change
        }
    }
}
