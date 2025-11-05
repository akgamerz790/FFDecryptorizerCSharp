using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace CoreOSLabsUtils
{
    public class ProfileManager
    {
        public static string GetDefaultFirefoxProfilePath()
        {
            string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string firefoxPath = Path.Combine(appdata, "Mozilla", "Firefox", "Profiles");

            if (!Directory.Exists(firefoxPath))
                throw new DirectoryNotFoundException("Firefox profiles directory not found.");

            // Return first profile found (simplification)
            string[] profiles = Directory.GetDirectories(firefoxPath);
            if (profiles.Length == 0)
                throw new Exception("No Firefox profiles found.");

            return profiles[0];
        }

        public static IEnumerable<LoginEntry> LoadLogins(string profilePath)
        {
            string loginJsonPath = Path.Combine(profilePath, "logins.json");
            if (!File.Exists(loginJsonPath))
                throw new FileNotFoundException("logins.json not found in profile.");

            string json = File.ReadAllText(loginJsonPath);
            var doc = JsonDocument.Parse(json);
            if(!doc.RootElement.TryGetProperty("logins", out var logins))
                throw new Exception("Invalid logins.json format.");

            foreach (var login in logins.EnumerateArray())
            {
                yield return new LoginEntry
                {
                    Hostname = login.GetProperty("hostname").GetString(),
                    EncryptedUsername = login.GetProperty("encryptedUsername").GetString(),
                    EncryptedPassword = login.GetProperty("encryptedPassword").GetString()
                };
            }
        }
    }

    public class LoginEntry
    {
        public string? Hostname { get; set; }
        public string? EncryptedUsername { get; set; }
        public string? EncryptedPassword { get; set; }
    }
}
