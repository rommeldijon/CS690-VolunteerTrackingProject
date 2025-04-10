using Xunit;
using System;
using System.IO;
using VolunteerTracking.Models;
using VolunteerTracking;

namespace VolunteerTracking.Tests
{
    public class ActivityEditorTests
    {
        [Fact]
        public void Edit_ShouldUpdateDate_WhenNewDateIsProvided()
        {
            // Arrange
            var activity = new Activity
            {
                Date = "01/01/2025"
            };

            // Simulate:
            // 1. Select "Date"
            // 2. Input new date "04/15/2025"
            // 3. Press Enter to continue
            var input = new StringReader("Date\n04/15/2025\n\n");
            Console.SetIn(input);

            // Redirect console output (optional: to suppress screen clutter)
            var originalOut = Console.Out;
            Console.SetOut(new StringWriter());

            // Act
            ActivityEditor.Edit(ref activity);

            // Assert
            Assert.Equal("04/15/2025", activity.Date);

            // Reset output
            Console.SetOut(originalOut);
        }

        [Fact]
        public void Edit_ShouldNotChangeAnything_WhenCanceled()
        {
            // Arrange
            var activity = new Activity
            {
                Organization = "Red Cross"
            };

            // Simulate selecting "Cancel Edit"
            var input = new StringReader("Cancel Edit\n\n");
            Console.SetIn(input);

            var originalOut = Console.Out;
            Console.SetOut(new StringWriter());

            // Act
            ActivityEditor.Edit(ref activity);

            // Assert
            Assert.Equal("Red Cross", activity.Organization);

            // Reset output
            Console.SetOut(originalOut);
        }

        [Fact]
        public void Edit_ShouldUpdateNote_WhenNewNoteIsProvided()
        {
            // Arrange
            var activity = new Activity
            {
                Note = "Old Note"
            };

            // Simulate:
            // 1. Select "Note"
            // 2. Enter "Updated Note"
            // 3. Press Enter
            var input = new StringReader("Note\nUpdated Note\n\n");
            Console.SetIn(input);

            var originalOut = Console.Out;
            Console.SetOut(new StringWriter());

            // Act
            ActivityEditor.Edit(ref activity);

            // Assert
            Assert.Equal("Updated Note", activity.Note);

            // Reset output
            Console.SetOut(originalOut);
        }
    }
}
