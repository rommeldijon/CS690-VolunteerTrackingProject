namespace VolunteerTracking;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spectre.Console;
using VolunteerTracking.Models;

public partial class Program
{
    static Volunteer Authenticate(string username, string password)
    {
        if (!File.Exists(filePath))
            return null;

        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var v = Volunteer.FromCsv(line);
            string hashed = Volunteer.HashPassword(password);
            if (v.Username.ToLower() == username && v.Password == hashed)
                return v;
        }

        return null;
    }
    static void ResetPassword()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine()?.ToLower();

        if (!File.Exists(filePath))
        {
            Console.WriteLine("No users found.");
            return;
        }

        var lines = File.ReadAllLines(filePath);
        bool found = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var v = Volunteer.FromCsv(lines[i]);
            if (v.Username.ToLower() == username)
            {
                Console.WriteLine("User verified. Let's reset your password.");

                string newPassword = "";
                while (true)
                {
                    Console.Write("New password (min 6 chars, 1 capital letter, 1 special character {!@#$%^&*()}): ");
                    string pass1 = Console.ReadLine();

                    Console.Write("Confirm new password: ");
                    string pass2 = Console.ReadLine();

                    if (pass1 != pass2)
                    {
                        Console.WriteLine("Passwords do not match. Try again.");
                        continue;
                    }

                    if (!Validator.IsPasswordValid(pass1))
                    {
                        Console.WriteLine("Password does not meet requirements. Try again.");
                        continue;
                    }

                    newPassword = Volunteer.HashPassword(pass1);
                    break;
                }

                v.Password = newPassword;
                lines[i] = v.ToString();
                found = true;
                break;
            }
        }

        if (found)
        {
            File.WriteAllLines(filePath, lines);
            Console.WriteLine("Password successfully reset.");
        }
        else
        {
            Console.WriteLine("User not found or information does not match.");
        }
    }
}
