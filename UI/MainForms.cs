using System;
using System.Windows.Forms;
using CoreOSLabsUtils;

namespace CoreOSLabsUtils.UI
{
    public class MainForm : Form
    {
        private Button btnSelectProfile;
        private Button btnDecrypt;
        private ListView listViewPasswords;

        private string SelectedProfilePath = null;
        private FirefoxDecryptor Decryptor;

        public MainForm()
        {
            this.Text = "Firefox Password Decryptor";
            this.Width = 800;
            this.Height = 600;

            btnSelectProfile = new Button() { Text = "Select Profile", Left = 10, Top = 10, Width = 120 };
            btnSelectProfile.Click += BtnSelectProfile_Click;

            btnDecrypt = new Button() { Text = "Decrypt Passwords", Left = 150, Top = 10, Width = 150 };
            btnDecrypt.Click += BtnDecrypt_Click;
            btnDecrypt.Enabled = false;

            listViewPasswords = new ListView()
            {
                Left = 10,
                Top = 50,
                Width = 760,
                Height = 500,
                View = View.Details,
                FullRowSelect = true
            };

            listViewPasswords.Columns.Add("Hostname", 300);
            listViewPasswords.Columns.Add("Username", 200);
            listViewPasswords.Columns.Add("Password", 240);

            this.Controls.Add(btnSelectProfile);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(listViewPasswords);
        }

        private void BtnSelectProfile_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedProfilePath = fbd.SelectedPath;
                    btnDecrypt.Enabled = true;
                }
            }
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
                Decryptor = new FirefoxDecryptor();

                if (!Decryptor.Initialize(SelectedProfilePath))
                {
                    MessageBox.Show("Failed to initialize NSS. Check profile path.");
                    return;
                }

                listViewPasswords.Items.Clear();

                foreach (var (host, user, pass) in Decryptor.DecryptPasswords(SelectedProfilePath))
                {
                    var item = new ListViewItem(new[] { host, user, pass });
                    listViewPasswords.Items.Add(item);
                }

                Decryptor.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decrypting passwords: " + ex.Message);
            }
        }
    }
}
