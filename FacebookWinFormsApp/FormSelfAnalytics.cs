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
        private UiPalette m_Palette;

        public FormSelfAnalytics(LoginResult i_LoginResult, UiPalette i_Palette)
            : this()
        {
            m_LoginResult = i_LoginResult;
            m_Palette = i_Palette;
        }

        public FormSelfAnalytics(LoginResult i_LoginResult)
            : this()
        {
            m_LoginResult = i_LoginResult;
            m_Palette = new UiPalette();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            applyDarkMode();

            if (!isLoggedIn())
            {
                MessageBox.Show("No logged-in user. Open this form with new FormSelfAnalytics(m_LoginResult, palette).");
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

        public void SetPalette(UiPalette i_Palette)
        {
            m_Palette = i_Palette ?? m_Palette;
            applyDarkMode();
        }

        private bool isLoggedIn()
        {
            return m_LoginResult != null && m_LoginResult.LoggedInUser != null;
        }

        private void applyDarkMode()
        {
            Color formBack = r_IsDarkMode ? ColorPalette.sr_Black : SystemColors.Control;
            Color panelBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;

            Color primaryText = r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;
            Color secondaryText = r_IsDarkMode ? ColorPalette.sr_LightGray : ColorPalette.sr_Black;
            Color mutedText = r_IsDarkMode ? ColorPalette.sr_LightGray : ColorPalette.sr_MidGray;

            Color listBack = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color listFore = r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_DarkBlue;

            BackColor = p.FormBack;

            if (labelName != null) labelName.ForeColor = p.PrimaryText;
            if (labelSubtitle != null) labelSubtitle.ForeColor = p.MutedText;
            if (labelBirthday != null) labelBirthday.ForeColor = p.SecondaryText;
            if (labelGender != null) labelGender.ForeColor = p.SecondaryText;
            if (labelStats != null) labelStats.ForeColor = p.StatsText;

            if (listBoxFriends != null)
            {
                listBoxFriends.BackColor = p.ListBack;
                listBoxFriends.ForeColor = p.ListFore;
            }

            if (pictureBoxProfile != null)
            {
                pictureBoxProfile.BackColor = p.ProfileBack;
            }

            if(labelGender != null)
            {
                labelGender.ForeColor = secondaryText;
            }

            if(labelStats != null)
            {
                labelStats.ForeColor = r_IsDarkMode ? ColorPalette.sr_WhitishBlue : ColorPalette.sr_Black;
            }

            if(listBoxFriends != null)
            {
                listBoxFriends.BackColor = listBack;
                listBoxFriends.ForeColor = listFore;
            }

            if(pictureBoxProfile != null)
            {
                pictureBoxProfile.BackColor = panelBack;
            }

            if(buttonBack != null)
            {
                buttonBack.ForeColor = ColorPalette.sr_White;
                buttonBack.BackColor = r_IsDarkMode ? ColorPalette.sr_DarkBlue : ColorPalette.sr_FacebookBlue;
                buttonBack.FlatStyle = FlatStyle.Flat;
            }

            if(panelCard != null)
            {
                panelCard.BackColor = panelBack;
                panelCard.Invalidate();
            }
        }

        private void populateAnalytics()
        {
            User user = m_LoginResult.LoggedInUser;

            loadProfilePicture(user);
            setMainLabels(user);
            setStatsBlock(user);
            fillFriendsList(user);

            panelCard.Invalidate();
        }

        private void loadProfilePicture(User i_User)
        {
            try
            {
                if(!string.IsNullOrEmpty(i_User.PictureNormalURL))
                {
                    pictureBoxProfile.LoadAsync(i_User.PictureNormalURL);
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
        }

        private void setMainLabels(User i_User)
        {
            labelName.Text = i_User.Name ?? "(name unavailable)";

            DateTime birthday;
            if(!string.IsNullOrEmpty(i_User.Birthday) && DateTime.TryParse(i_User.Birthday, out birthday))
            {
                int age = calculateAge(birthday, DateTime.Today);
                labelBirthday.Text = $"{birthday:d}  •  Age: {age}";
            }
            else
            {
                labelBirthday.Text = "(birthday unavailable)";
            }

            labelGender.Text = $"Gender: {i_User.Gender?.ToString() ?? "(not available)"}";
        }

        private void setStatsBlock(User i_User)
        {
            string stats = buildStatsText(i_User);
            labelStats.Text = stats.Replace(Environment.NewLine, Environment.NewLine + Environment.NewLine);
        }

        private string buildStatsText(User i_User)
        {
            StringBuilder statsSb = new StringBuilder();

            int albumsCount = i_User.Albums?.Count() ?? 0;
            int postsCount = i_User.Posts?.Count() ?? 0;
            int photosCount = i_User.PhotosTaggedIn?.Count() ?? 0;

            statsSb.AppendLine($"Albums: {albumsCount}");
            statsSb.AppendLine($"Posts: {postsCount}");
            statsSb.AppendLine($"Tagged Photos: {photosCount}");

            int? earliestYear = getEarliestPostYear(i_User);
            if(earliestYear.HasValue)
            {
                statsSb.AppendLine($"Joined (approx): {earliestYear.Value}");
            }
            else
            {
                statsSb.AppendLine("Joined (approx): (not available)");
            }

            DateTime birthday;
            if(i_User.Posts != null && i_User.Posts.Any() && !string.IsNullOrEmpty(i_User.Birthday)
               && DateTime.TryParse(i_User.Birthday, out birthday))
            {
                string[] decadeGroups = getPostsByDecade(i_User, birthday);
                if(decadeGroups.Length > 0)
                {
                    statsSb.AppendLine();
                    statsSb.AppendLine("Posts by decade:");
                    foreach(string item in decadeGroups)
                    {
                        statsSb.AppendLine(item);
                    }
                }
            }

            return statsSb.ToString();
        }

        private int? getEarliestPostYear(User i_User)
        {
            DateTime? earliestPostDate = null;
            try
            {
                earliestPostDate = i_User.Posts?.Where(i_Post => i_Post != null && i_Post.CreatedTime.HasValue)
                    .Select(i_Post => i_Post.CreatedTime).OrderBy(d => d.Value).FirstOrDefault();
            }
            catch
            {
                earliestPostDate = null;
            }

            return earliestPostDate.HasValue ? (int?)earliestPostDate.Value.Year : null;
        }

        private string[] getPostsByDecade(User i_User, DateTime i_Birthday)
        {
            return i_User.Posts.Where(i_Post => i_Post != null && i_Post.CreatedTime.HasValue).Select(p =>
                    {
                        DateTime created = p.CreatedTime.Value;
                        int ageAtPost = calculateAge(i_Birthday, created);
                        int decadeFloor = ageAtPost / 10 * 10;
                        string decadeLabel = decadeFloor < 0 ? "Unknown" : $"{decadeFloor}s";
                        return decadeLabel;
                    }).GroupBy(d => d).OrderBy(i_Grouping => i_Grouping.Key)
                .Select(i_Grouping => $"{i_Grouping.Key}: {i_Grouping.Count()}").ToArray();
        }

        private void fillFriendsList(User i_User)
        {
            listBoxFriends.BeginUpdate();
            listBoxFriends.Items.Clear();
            try
            {
                string[] friendNames = i_User.Friends?.Where(i_User1 => i_User1 != null).Select(i_User1 => i_User1.Name)
                    .Where(i_Value => !string.IsNullOrEmpty(i_Value)).OrderBy(n => n).Take(12).ToArray();

                if(friendNames == null)
                {
                    listBoxFriends.Items.Add("(friends not available - check permission)");
                }
                else if(friendNames.Length == 0)
                {
                    listBoxFriends.Items.Add("(no friend names available)");
                }
                else
                {
                    foreach(string item in friendNames)
                    {
                        listBoxFriends.Items.Add(item);
                    }

                    if((i_User.Friends?.Count() ?? 0) > friendNames.Length)
                    {
                        listBoxFriends.Items.Add($"...and {i_User.Friends.Count() - friendNames.Length} more");
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
        }

        private int calculateAge(DateTime i_Birthday, DateTime i_DateTime)
        {
            int age = i_DateTime.Year - i_Birthday.Year;
            if(i_DateTime < i_Birthday.AddYears(age))
            {
                age--;
            }

            return age;
        }

        private void panelCard_Paint(object sender, PaintEventArgs e)
        {
            var p = m_Palette ?? new UiPalette();

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = panelCard.ClientRectangle;
            rect.Inflate(-2, -2);

            Color outerStart = r_IsDarkMode ? ColorPalette.sr_DarkBlue : ColorPalette.sr_WhitishBlue;
            Color outerEnd = r_IsDarkMode ? ColorPalette.sr_Black : ColorPalette.sr_DarkBlue;

            Color innerTop = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_White;
            Color innerBottom = r_IsDarkMode ? ColorPalette.sr_DarkGray : ColorPalette.sr_WhitishBlue;
            Color innerBorder = r_IsDarkMode ? ColorPalette.sr_LightGray : ColorPalette.sr_MidGray;

            Color shineStart = r_IsDarkMode ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(60, 255, 255, 255);
            Color shineEnd = r_IsDarkMode ? Color.FromArgb(5, 255, 255, 255) : Color.FromArgb(10, 255, 255, 255);

            int radius = 18;
            using (var path = new GraphicsPath())
            {
                int d = radius * 2;
                path.AddArc(rect.Left, rect.Top, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();

                using (var br = new LinearGradientBrush(rect, p.CardOuterStart, p.CardOuterEnd, LinearGradientMode.Vertical))
                {
                    g.FillPath(br, path);
                }

                var innerRect = Rectangle.Inflate(rect, -8, -8);
                using (var innerPath = RoundedRect(innerRect, 12))
                using (var br2 = new LinearGradientBrush(innerRect, p.CardInnerTop, p.CardInnerBottom, LinearGradientMode.Vertical))
                {
                    g.FillPath(br2, innerPath);
                    using (var pen = new Pen(p.CardInnerBorder))
                    {
                        g.DrawPath(pen, innerPath);
                    }
                }

                using (var shine = new LinearGradientBrush(
                           new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height / 3),
                           p.CardShineStart, p.CardShineEnd, LinearGradientMode.Vertical))
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

        private void buttonBack_Click(object i_Sender, EventArgs i_EventArgs)
        {
            Close();
        }
    }
}