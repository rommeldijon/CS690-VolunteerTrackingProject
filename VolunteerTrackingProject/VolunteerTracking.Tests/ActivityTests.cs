using Xunit;
using VolunteerTracking.Models;
using System;

namespace VolunteerTracking.Tests
{
    public class ActivityTests
    {
        [Fact]
        public void FromCsv_ShouldParseValidCsvLine()
        {
            // Arrange
            string csv = "jdoe,Red Cross,123 Main St,456 Elm St,04/01/2025,9:00 AM,12:00 PM,Food Drive,Helped pack boxes";

            // Act
            Activity activity = Activity.FromCsv(csv);

            // Assert
            Assert.Equal("jdoe", activity.Username);
            Assert.Equal("Red Cross", activity.Organization);
            Assert.Equal("123 Main St", activity.Address);
            Assert.Equal("456 Elm St", activity.Location);
            Assert.Equal("04/01/2025", activity.Date);
            Assert.Equal("9:00 AM", activity.StartTime);
            Assert.Equal("12:00 PM", activity.EndTime);
            Assert.Equal("Food Drive", activity.Type);
            Assert.Equal("Helped pack boxes", activity.Note);
        }

        [Fact]
        public void ToString_ShouldReturnCsvFormat()
        {
            // Arrange
            var activity = new Activity
            {
                Username = "jdoe",
                Organization = "Red Cross",
                Address = "123 Main St",
                Location = "456 Elm St",
                Date = "04/01/2025",
                StartTime = "9:00 AM",
                EndTime = "12:00 PM",
                Type = "Food Drive",
                Note = "Helped pack boxes"
            };

            // Act
            string csv = activity.ToString();

            // Assert
            string expected = "jdoe,Red Cross,123 Main St,456 Elm St,04/01/2025,9:00 AM,12:00 PM,Food Drive,Helped pack boxes";
            Assert.Equal(expected, csv);
        }

        [Fact]
        public void FromCsv_ShouldThrowException_OnEmptyLine()
        {
            // Arrange
            string csv = "";

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => Activity.FromCsv(csv));
        }

        [Fact]
        public void FromCsv_ShouldThrowException_OnMalformedCsv()
        {
            // Arrange: only 5 fields instead of expected 9
            string csv = "jdoe,Red Cross,123 Main St,456 Elm St,04/01/2025";

            // Act & Assert
            Assert.Throws<IndexOutOfRangeException>(() => Activity.FromCsv(csv));
        }

        [Fact]
        public void ToString_ShouldHandleEmptyNote()
        {
            // Arrange
            var activity = new Activity
            {
                Username = "jdoe",
                Organization = "Red Cross",
                Address = "123 Main St",
                Location = "456 Elm St",
                Date = "04/01/2025",
                StartTime = "9:00 AM",
                EndTime = "12:00 PM",
                Type = "Food Drive",
                Note = ""
            };

            // Act
            string csv = activity.ToString();

            // Assert
            string expected = "jdoe,Red Cross,123 Main St,456 Elm St,04/01/2025,9:00 AM,12:00 PM,Food Drive,";
            Assert.Equal(expected, csv);
        }
    }
}
