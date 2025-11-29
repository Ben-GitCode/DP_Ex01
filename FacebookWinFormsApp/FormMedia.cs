using FacebookWrapper;
using System;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormMedia : Form
    {
        private LoginResult m_LoginResult;
        private bool m_IsDarkMode = false;

        public FormMedia(LoginResult loginResult, bool isDarkMode)
        {
            InitializeComponent();
            m_LoginResult = loginResult;
            m_IsDarkMode = isDarkMode;
        }

    }
}
