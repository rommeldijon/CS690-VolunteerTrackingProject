namespace VolunteerTracking;

public static class Validator
{
    public static bool IsPasswordValid(string password)
    {
        if (password.Length < 6)
            return false;

        bool hasUpper = false;
        bool hasSpecial = false;
        string specialChars = "!@#$%^&*()";

        foreach (char c in password)
        {
            if (char.IsUpper(c)) hasUpper = true;
            if (specialChars.Contains(c)) hasSpecial = true;
        }

        return hasUpper && hasSpecial;
    }

    public static bool IsUsernameValid(string username)
    {
        if (string.IsNullOrWhiteSpace(username) || username.Length < 6)
            return false;

        foreach (char c in username)
        {
            if (!char.IsLetterOrDigit(c) && c != ' ')
                return false;
        }

        return true;
    }
}
