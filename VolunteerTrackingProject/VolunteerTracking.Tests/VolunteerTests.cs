using Xunit;
using VolunteerTracking.Models;

namespace VolunteerTracking.Tests
{
    public class VolunteerTests
    {
        [Fact]
        public void HashPassword_ShouldReturnNonEmptyString()
        {
            // Arrange
            string password = "MySecurePassword123!";

            // Act
            string hashed = Volunteer.HashPassword(password);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(hashed));
        }

        [Fact]
        public void FromCsv_ShouldParseValidCsvLine()
        {
            // Arrange
            string csv = "jdoe,73e87ed2b16a0b9ae8096fc0347b368e9c62a69799bdc2a91a3b1cf474b01418,John Doe";

            // Act
            Volunteer volunteer = Volunteer.FromCsv(csv);

            // Assert
            Assert.Equal("jdoe", volunteer.Username);
            Assert.Equal("John Doe", volunteer.FullName);
            Assert.Equal("73e87ed2b16a0b9ae8096fc0347b368e9c62a69799bdc2a91a3b1cf474b01418", volunteer.Password);
        }

        [Fact]
        public void ToString_ShouldReturnCsvFormat()
        {
            // Arrange
            var volunteer = new Volunteer
            {
                Username = "jdoe",
                FullName = "John Doe",
                Password = "73e87ed2b16a0b9ae8096fc0347b368e9c62a69799bdc2a91a3b1cf474b01418"
            };

            // Act
            string result = volunteer.ToString();

            // Assert
            string expected = "jdoe,73e87ed2b16a0b9ae8096fc0347b368e9c62a69799bdc2a91a3b1cf474b01418,John Doe";
            Assert.Equal(expected, result);
        }


        [Fact]
        public void HashPassword_ShouldReturnConsistentHash()
        {
            // Act
            string hash1 = Volunteer.HashPassword("Secure123!");
            string hash2 = Volunteer.HashPassword("Secure123!");

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void HashPassword_ShouldProduceDifferentHashForDifferentInput()
        {
            // Act
            string hash1 = Volunteer.HashPassword("Secure123!");
            string hash2 = Volunteer.HashPassword("AnotherPass!");

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void FromCsv_ShouldThrowException_OnMalformedLine()
        {
            string invalidCsv = "jdoe,hashedpass"; // Missing FullName
            Assert.Throws<FormatException>(() => Volunteer.FromCsv(invalidCsv));
        }
    }
}

