namespace VolunteerTracking.Models
{
    public class Activity
    {
        public string Username { get; set; } // Who logged it
        public string Organization { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }  // ✅ New field

        public override string ToString()
        {
            return $"{Username},{Organization},{Address},{Location},{Date},{StartTime},{EndTime},{Type},{Note}";
        }

        public static Activity FromCsv(string line)
        {
            var parts = line.Split(',');
            return new Activity
            {
                Username = parts[0],
                Organization = parts[1],
                Address = parts[2],
                Location = parts[3],
                Date = parts[4],
                StartTime = parts[5],
                EndTime = parts[6],
                Type = parts[7],
                Note = parts.Length > 8 ? parts[8] : ""  
            };
        }
    }
}
