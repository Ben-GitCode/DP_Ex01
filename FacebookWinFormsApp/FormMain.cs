using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            FacebookWrapper.FacebookService.s_CollectionLimit = 25;
        }

        FacebookWrapper.LoginResult m_LoginResult;

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("design.patterns");

            if (m_LoginResult == null)
            {
                login();
            }
        }

        private void login()
        {
            m_LoginResult = FacebookService.Login(
                /// (This is Desig Patter's App ID. replace it with your own)
                textBoxAppID.Text,
                /// requested permissions:
                "email",
                "public_profile"
                /// add any relevant permissions
                );

            if (string.IsNullOrEmpty(m_LoginResult.ErrorMessage))
            {
                afterLogin();
            }
        }


        private void buttonConnectAsDesig_Click(object sender, EventArgs e)
        {
            try
            {
                m_LoginResult = FacebookService.Connect("EAAUm6cZC4eUEBPZCFs9rJRpwlUmdHcPvU1tUNkIyP37zRZCjSvfdHaW5t3xsOnUL0bEKHL8Snjk6AZC3O32KWEbaItglEnXWQ2zEMXHqsdfdv0ecXNs3hO69juHrZCfRN9FGvfuJZAXhP4Pm57DRRoDeB8De6ZABnfrRflh6zgPwnavpyHS3ZCYX1E6K1QLTHff5sAZDZD");

                afterLogin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(m_LoginResult.ErrorMessage, "Login Failed");
            }
        }

        private void afterLogin()
        {

            buttonLogin.Text = $"Logged in as {m_LoginResult.LoggedInUser.Name}";
            buttonLogin.BackColor = Color.LightGreen;
            pictureBoxProfile.ImageLocation = m_LoginResult.LoggedInUser.PictureNormalURL;
            buttonLogin.Enabled = false;
            buttonLogout.Enabled = true;
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            FacebookService.LogoutWithUI();
            buttonLogin.Text = "Login";
            buttonLogin.BackColor = buttonLogout.BackColor;
            m_LoginResult = null;
            buttonLogin.Enabled = true;
            buttonLogout.Enabled = false;
        }
    }
}
