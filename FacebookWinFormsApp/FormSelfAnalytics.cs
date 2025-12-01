using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;

namespace BasicFacebookFeatures
{
    public partial class FormSelfAnalytics : Form
    {
        private LoginResult m_LoginResult;
        private bool m_IsDarkMode; // dark mode flag

        public FormSelfAnalytics(LoginResult i_LoginResult)
            : this()
        {
            m_LoginResult = i_LoginResult;
        }

        public FormSelfAnalytics(LoginResult i_LoginResult, bool i_IsDarkMode)
            : this(i_LoginResult)
        {
            m_IsDarkMode = i_IsDarkMode;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Apply theme first so controls render correctly
            applyDarkMode();

            if (m_LoginResult == null || m_LoginResult.LoggedInUser == null)
            {
                MessageBox.Show("No logged-in user. Open this form with new FormSelfAnalytics(m_LoginResult).");
                return;
            }

            try
            {
                populateAnalytics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to compute analytics: " + ex.Message);
            }
        }

        // Allow toggling after construction if needed
        public void SetDarkMode(bool i_IsDarkMode)
        {
            m_IsDarkMode = i_IsDarkMode;
            applyDarkMode();
        }

        private void applyDarkMode()
        {
            Color formBack = m_IsDarkMode ? Color.FromArgb(24, 25, 26) : SystemColors.Control;
            Color primaryText = m_IsDarkMode ? Color.White : Color.FromArgb(12, 36, 86);
            Color secondaryText = m_IsDarkMode ? Color.Gainsboro : Color.FromArgb(34, 34, 34);
            Color mutedText = m_IsDarkMode ? Color.Silver : Color.FromArgb(90, 90, 110);
            Color listBack = m_IsDarkMode ? Color.FromArgb(36, 37, 38) : Color.WhiteSmoke;
            Color listFore = m_IsDarkMode ? Color.White : Color.Black;

            this.BackColor = formBack;

            // Labels and list colors
            if (labelName != null) labelName.ForeColor = primaryText;
            if (labelSubtitle != null) labelSubtitle.ForeColor = mutedText;
            if (labelBirthday != null) labelBirthday.ForeColor = secondaryText;
            if (labelGender != null) labelGender.ForeColor = secondaryText;
            if (labelStats != null) labelStats.ForeColor = m_IsDarkMode ? Color.Gainsboro : Color.FromArgb(40, 40, 40);

            if (listBoxFriends != null)
            {
                listBoxFriends.BackColor = listBack;
                listBoxFriends.ForeColor = listFore;
            }

            if (pictureBoxProfile != null)
            {
                pictureBoxProfile.BackColor = m_IsDarkMode ? Color.FromArgb(36, 37, 38) : Color.White;
            }

            // Keep button style readable
            if (buttonBack != null)
            {
                buttonBack.ForeColor = Color.White;
                if (buttonBack.BackColor == SystemColors.Control || buttonBack.BackColor.A == 0)
                {
                    buttonBack.BackColor = Color.FromArgb(66, 103, 178);
                }
                buttonBack.FlatStyle = FlatStyle.Flat;
            }

            // Repaint the card using dark palette if needed
            if (panelCard != null)
            {
                panelCard.Invalidate();
            }
        }

        private void populateAnalytics()
        {
            var user = m_LoginResult.LoggedInUser;

            // Profile picture
            try
            {
                if (!string.IsNullOrEmpty(user.PictureNormalURL))
                {
                    pictureBoxProfile.LoadAsync(user.PictureNormalURL);
                }
                else
                {
                    pictureBoxProfile.Image = null;
                }
            }
            catch
            {
                pictureBoxProfile.Image = null;
            }

            // Main labels
            labelName.Text = user.Name ?? "(name unavailable)";

            DateTime birthday;
            if (!string.IsNullOrEmpty(user.Birthday) && DateTime.TryParse(user.Birthday, out birthday))
            {
                int age = calculateAge(birthday, DateTime.Today);
                labelBirthday.Text = $"{birthday:d}  •  Age: {age}";
            }
            else
            {
                labelBirthday.Text = "(birthday unavailable)";
            }

            labelGender.Text = $"Gender: {user.Gender?.ToString() ?? "(not available)"}";

            // Stats panel
            int albumsCount = user.Albums?.Count() ?? 0;
            int postsCount = user.Posts?.Count() ?? 0;
            int photosCount = user.PhotosTaggedIn?.Count() ?? 0;

            var statsSb = new StringBuilder();
            statsSb.AppendLine($"Albums: {albumsCount}");
            statsSb.AppendLine($"Posts: {postsCount}");
            statsSb.AppendLine($"Tagged Photos: {photosCount}");

            DateTime? earliestPostDate = null;
            try
            {
                earliestPostDate = user.Posts?
                    .Where(p => p != null && p.CreatedTime.HasValue)
                    .Select(p => (DateTime?)p.CreatedTime)
                    .OrderBy(d => d.Value)
                    .FirstOrDefault();
            }
            catch
            {
                earliestPostDate = null;
            }

            if (earliestPostDate.HasValue)
            {
                statsSb.AppendLine($"Joined (approx): {earliestPostDate.Value.Year}");
            }
            else
            {
                statsSb.AppendLine("Joined (approx): (not available)");
            }

            // Posts by decade (short summary)
            if (user.Posts != null && user.Posts.Any() && DateTime.TryParse(user.Birthday, out birthday))
            {
                var decadeGroups = user.Posts
                    .Where(p => p != null && p.CreatedTime.HasValue)
                    .Select(p =>
                    {
                        var created = p.CreatedTime.Value;
                        int ageAtPost = calculateAge(birthday, created);
                        int decadeFloor = (ageAtPost / 10) * 10;
                        string decadeLabel = decadeFloor < 0 ? "Unknown" : $"{decadeFloor}s";
                        return decadeLabel;
                    })
                    .GroupBy(d => d)
                    .OrderBy(g => g.Key)
                    .Select(g => $"{g.Key}: {g.Count()}")
                    .ToArray();

                if (decadeGroups.Length > 0)
                {
                    statsSb.AppendLine();
                    statsSb.AppendLine("Posts by decade:");
                    foreach (var item in decadeGroups)
                    {
                        statsSb.AppendLine(item);
                    }
                }
            }

            // Increase spacing between lines by doubling newlines (minimal, no other UI changes)
            // Adjust the Environment.NewLine duplication if you prefer tighter/looser spacing.
            labelStats.Text = statsSb.ToString().Replace(Environment.NewLine, Environment.NewLine + Environment.NewLine);

            // Friends list (show up to 12)
            listBoxFriends.BeginUpdate();
            listBoxFriends.Items.Clear();
            try
            {
                var friendNames = user.Friends?
                    .Where(f => f != null)
                    .Select(f => f.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .OrderBy(n => n)
                    .Take(12)
                    .ToArray();

                if (friendNames == null)
                {
                    listBoxFriends.Items.Add("(friends not available - check permission)");
                }
                else if (friendNames.Length == 0)
                {
                    listBoxFriends.Items.Add("(no friend names available)");
                }
                else
                {
                    foreach (var fn in friendNames)
                    {
                        listBoxFriends.Items.Add(fn);
                    }

                    if ((user.Friends?.Count() ?? 0) > friendNames.Length)
                    {
                        listBoxFriends.Items.Add($"...and {(user.Friends.Count() - friendNames.Length)} more");
                    }
                }
            }
            catch
            {
                listBoxFriends.Items.Add("(failed to load friends)");
            }
            finally
            {
                listBoxFriends.EndUpdate();
            }

            // Force card redraw (so gradient/highlight updates)
            panelCard.Invalidate();
        }

        private int calculateAge(DateTime birthday, DateTime atDate)
        {
            int age = atDate.Year - birthday.Year;
            if (atDate < birthday.AddYears(age))
            {
                age--;
            }
            return age;
        }

        private void panelCard_Paint(object sender, PaintEventArgs e)
        {
            // Draw rounded rectangle with gradient and subtle highlight for an "ID card" look
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = panelCard.ClientRectangle;
            rect.Inflate(-2, -2);

            // Dark/light palettes
            Color outerStart = m_IsDarkMode ? Color.FromArgb(30, 40, 60) : Color.FromArgb(40, 83, 155);
            Color outerEnd = m_IsDarkMode ? Color.FromArgb(16, 22, 33) : Color.FromArgb(14, 36, 86);

            Color innerTop = m_IsDarkMode ? Color.FromArgb(44, 47, 51) : Color.FromArgb(255, 255, 255, 255);
            Color innerBottom = m_IsDarkMode ? Color.FromArgb(36, 37, 38) : Color.FromArgb(240, 240, 246);
            Color innerBorder = m_IsDarkMode ? Color.FromArgb(80, 80, 80) : Color.FromArgb(200, 200, 200);

            Color shineStart = m_IsDarkMode ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(60, 255, 255, 255);
            Color shineEnd = m_IsDarkMode ? Color.FromArgb(5, 255, 255, 255) : Color.FromArgb(10, 255, 255, 255);

            int radius = 18;
            using (var path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();

                using (var br = new LinearGradientBrush(rect, outerStart, outerEnd, LinearGradientMode.Vertical))
                {
                    g.FillPath(br, path);
                }

                // Inner panel overlay
                var innerRect = Rectangle.Inflate(rect, -8, -8);
                using (var innerPath = RoundedRect(innerRect, 12))
                using (var br2 = new LinearGradientBrush(innerRect, innerTop, innerBottom, LinearGradientMode.Vertical))
                {
                    g.FillPath(br2, innerPath);
                    using (var pen = new Pen(innerBorder))
                    {
                        g.DrawPath(pen, innerPath);
                    }
                }

                // subtle shine
                using (var shine = new LinearGradientBrush(new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height / 3), shineStart, shineEnd, LinearGradientMode.Vertical))
                {
                    g.FillPath(shine, path);
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var gp = new GraphicsPath();
            int d = radius * 2;
            gp.AddArc(r.Left, r.Top, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            try
            {
                var main = Application.OpenForms.OfType<FormMain>().FirstOrDefault();
                if (main != null)
                {
                    var mi = main.GetType().GetMethod("navigateToMenu", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    if (mi != null)
                    {
                        mi.Invoke(main, null);
                        this.Close();
                        return;
                    }

                    main.Invoke(new Action(() => main.BringToFront()));
                }
            }
            catch
            {
            }

            this.Close();
        }
    }
}