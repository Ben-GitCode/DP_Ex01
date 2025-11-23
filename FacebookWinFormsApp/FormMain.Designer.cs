using System;
using System.Drawing;
using System.Windows.Forms;

namespace BasicFacebookFeatures
{
    partial class FormMain : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Button buttonLogin;
        private Button buttonLogout;
        private Label label1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TextBox textBoxAppID;
        private PictureBox pictureBoxProfile;
        private Button buttonConnectAsDesig;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.pictureBoxProfile = new System.Windows.Forms.PictureBox();
            this.textBoxAppID = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonConnectAsDesig = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).BeginInit();
            this.SuspendLayout();

            // FormMain Styling
            this.Font = new Font("Segoe UI", 11F);
            this.BackColor = Color.FromArgb(59, 89, 152); // Facebook blue

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new Point(314, 17);
            this.label1.Name = "label1";
            this.label1.Size = new Size(465, 78);
            this.label1.TabIndex = 53;
            this.label1.Text = "This is the AppID of 'Design Patterns App 2.4'.\r\nThe grader will use it to test your app.\r\nType here your own AppID to test it:";
            this.label1.ForeColor = Color.White;
            this.label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);

            // tabControl1
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Location = new Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(862, 370);
            this.tabControl1.TabIndex = 54;

            // tabPage1
            this.tabPage1.Controls.Add(this.buttonConnectAsDesig);
            this.tabPage1.Controls.Add(this.pictureBoxProfile);
            this.tabPage1.Controls.Add(this.textBoxAppID);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.buttonLogout);
            this.tabPage1.Controls.Add(this.buttonLogin);
            this.tabPage1.Location = new Point(4, 35);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new Padding(3);
            this.tabPage1.Size = new Size(854, 331);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Welcome";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.BackColor = Color.White;

            // tabPage2
            this.tabPage2.Location = new Point(4, 35);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new Size(752, 327);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Other";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.BackColor = Color.White;

            // buttonLogin
            this.buttonLogin.Location = new Point(18, 17);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new Size(268, 44);
            this.buttonLogin.TabIndex = 36;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.FlatStyle = FlatStyle.Flat;
            this.buttonLogin.BackColor = Color.White;
            this.buttonLogin.ForeColor = Color.FromArgb(59, 89, 152);
            this.buttonLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new EventHandler(this.buttonLogin_Click);

            // buttonConnectAsDesig
            this.buttonConnectAsDesig.Location = new Point(18, 69);
            this.buttonConnectAsDesig.Name = "buttonConnectAsDesig";
            this.buttonConnectAsDesig.Size = new Size(268, 44);
            this.buttonConnectAsDesig.TabIndex = 56;
            this.buttonConnectAsDesig.Text = "Connect As Desig";
            this.buttonConnectAsDesig.FlatStyle = FlatStyle.Flat;
            this.buttonConnectAsDesig.BackColor = Color.LightSkyBlue;
            this.buttonConnectAsDesig.ForeColor = Color.White;
            this.buttonConnectAsDesig.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.buttonConnectAsDesig.UseVisualStyleBackColor = true;
            this.buttonConnectAsDesig.Click += new EventHandler(this.buttonConnectAsDesig_Click);

            // buttonLogout
            this.buttonLogout.Enabled = false;
            this.buttonLogout.Location = new Point(18, 121);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new Size(268, 43);
            this.buttonLogout.TabIndex = 52;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.FlatStyle = FlatStyle.Flat;
            this.buttonLogout.BackColor = Color.WhiteSmoke;
            this.buttonLogout.ForeColor = Color.Gray;
            this.buttonLogout.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new EventHandler(this.buttonLogout_Click);

            // pictureBoxProfile
            this.pictureBoxProfile.Location = new Point(18, 171);
            this.pictureBoxProfile.Name = "pictureBoxProfile";
            this.pictureBoxProfile.Size = new Size(79, 78);
            this.pictureBoxProfile.SizeMode = PictureBoxSizeMode.Zoom;
            this.pictureBoxProfile.TabIndex = 55;
            this.pictureBoxProfile.TabStop = false;
            this.pictureBoxProfile.BackColor = Color.White;
            this.pictureBoxProfile.BorderStyle = BorderStyle.FixedSingle;

            // textBoxAppID
            this.textBoxAppID.Location = new Point(319, 126);
            this.textBoxAppID.Name = "textBoxAppID";
            this.textBoxAppID.Size = new Size(446, 32);
            this.textBoxAppID.TabIndex = 54;
            this.textBoxAppID.Text = "1282715863617766";
            this.textBoxAppID.BackColor = Color.WhiteSmoke;
            this.textBoxAppID.Font = new Font("Segoe UI", 12F);

            // FormMain
            this.AutoScaleDimensions = new SizeF(13F, 26F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(862, 370);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Facebook Features";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProfile)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
