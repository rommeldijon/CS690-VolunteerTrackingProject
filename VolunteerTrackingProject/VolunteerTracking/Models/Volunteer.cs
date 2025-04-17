namespace VolunteerTracking.Models
{
    public class Volunteer
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        public override string ToString()
        {
            return $"{Username},{Password},{FullName}";
        }

        public static Volunteer FromCsv(string line)
        {
            var parts = line.Split(',');
            return new Volunteer
            {
                Username = parts[0],
                Password = parts[1],
                FullName = parts[2]
            };
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}