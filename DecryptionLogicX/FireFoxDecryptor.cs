using System;
using System.Collections.Generic;

namespace CoreOSLabsUtils
{
    public class FirefoxDecryptor
    {
        private NSSWrapper _nss;

        public FirefoxDecryptor()
        {
            _nss = new NSSWrapper();
        }

        public bool Initialize(string profilePath)
        {
            return _nss.Initialize(profilePath);
        }

        public void Shutdown()
        {
            _nss.Shutdown();
        }

        public IEnumerable<(string Hostname, string Username, string Password)> DecryptPasswords(string profilePath)
        {
            var logins = ProfileManager.LoadLogins(profilePath);

            foreach(var login in logins)
            {
                //Fix CS8604
                string? username = _nss.Decrypt(login.EncryptedUsername);
                string? password = _nss.Decrypt(login.EncryptedPassword);
                //Fix CS8619
                yield return (login.Hostname, username, password);
            }
        }
    }
}
