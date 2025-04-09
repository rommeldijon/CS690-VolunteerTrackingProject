using Xunit;
using System.Collections.Generic;
using VolunteerTracking.Models;
using System;
using System.Linq;
using System.IO;

namespace VolunteerTracking.Tests
{
    public class UtilsTests
    {
        [Theory]
        [InlineData("9", "9:00")]
        [InlineData("930", "9:30")]
        [InlineData("1130", "11:30")]
        [InlineData("10:00", "10:00")]
        public void NormalizeTime_ShouldFormatTimeCorrectly(string input, string expected)
        {
            string result = Utils.NormalizeTime(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SortActivitiesChronologically_ShouldReturnActivitiesInOrder()
        {
            var activities = new List<Activity>
            {
                new Activity { Date = "04/02/2025", StartTime = "11:00 AM" },
                new Activity { Date = "04/01/2025", StartTime = "9:00 AM" },
                new Activity { Date = "04/01/2025", StartTime = "2:00 PM" }
            };

            var sorted = Utils.SortActivitiesChronologically(activities);

            Assert.Equal("04/01/2025", sorted[0].Date);
            Assert.Equal("9:00 AM", sorted[0].StartTime);
            Assert.Equal("04/01/2025", sorted[1].Date);
            Assert.Equal("2:00 PM", sorted[1].StartTime);
            Assert.Equal("04/02/2025", sorted[2].Date);
            Assert.Equal("11:00 AM", sorted[2].StartTime);
        }

        [Fact]
        public void GetValidatedDate_ValidDate_ReturnsDate()
        {
            using (var input = new StringReader("04/15/2025\n"))
            {
                Console.SetIn(input);
                string result = Utils.GetValidatedDate("Enter date: ");
                Assert.Equal("04/15/2025", result);
            }
        }

        [Fact]
        public void GetValidatedTime_ValidTime_ReturnsFormattedTime()
        {
            using (var input = new StringReader("930\n"))
            {
                Console.SetIn(input);
                string result = Utils.GetValidatedTime("Enter time: ");
                Assert.Equal("9:30", result);
            }
        }

        [Fact]
        public void GetValidatedDate_Exit_ThrowsOperationCanceledException()
        {
            using (var input = new StringReader("exit\n"))
            {
                Console.SetIn(input);
                Assert.Throws<OperationCanceledException>(() => Utils.GetValidatedDate("Enter date: "));
            }
        }

        [Fact]
        public void GetValidatedTime_Exit_ThrowsOperationCanceledException()
        {
            using (var input = new StringReader("exit\n"))
            {
                Console.SetIn(input);
                Assert.Throws<OperationCanceledException>(() => Utils.GetValidatedTime("Enter time: "));
            }
        }
    }
}
