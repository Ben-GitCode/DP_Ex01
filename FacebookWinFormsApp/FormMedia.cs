using System;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    public partial class FormMedia : Form
    {
        public FormMedia()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Media Viewer";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = new System.Drawing.Size(800, 600);
        }
    }
}
