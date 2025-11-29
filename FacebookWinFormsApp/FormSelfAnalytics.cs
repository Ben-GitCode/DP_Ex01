using System;
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

        public FormSelfAnalytics()
        {
            InitializeComponent();
        }

        public FormSelfAnalytics(LoginResult i_LoginResult) : this()
        {
            m_LoginResult = i_LoginResult;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (m_LoginResult == null || m_LoginResult.LoggedInUser == null)
            {
                MessageBox.Show("No logged-in user. Open this form with new FormSelfAnalytics(m_LoginResult).");
                return;
            }

            try
            {
                PopulateAnalytics();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to compute analytics: " + ex.Message);
            }
        }

        private void PopulateAnalytics()
        {
            var user = m_LoginResult.LoggedInUser;
            var sb = new StringBuilder();

            sb.AppendLine($"Name: {user.Name}");
            sb.AppendLine();

            DateTime birthday;
            if (!string.IsNullOrEmpty(user.Birthday) && DateTime.TryParse(user.Birthday, out birthday))
            {
                int age = CalculateAge(birthday, DateTime.Today);
                sb.AppendLine($"Birthday: {birthday:d}    Age: {age}");
            }
            else
            {
                sb.AppendLine("Birthday: (not available)");
            }

            sb.AppendLine("Official age range: (not available)");
            sb.AppendLine($"Gender: {user.Gender?.ToString() ?? "(not available)"}");
            sb.AppendLine();

            int albumsCount = user.Albums?.Count() ?? 0;
            int postsCount = user.Posts?.Count() ?? 0;
            int photosCount = user.PhotosTaggedIn?.Count() ?? 0;
            sb.AppendLine($"Albums: {albumsCount}");
            sb.AppendLine($"Posts: {postsCount}");
            sb.AppendLine($"Tagged Photos: {photosCount}");
            sb.AppendLine();

            DateTime? earliestPostDate = null;
            try
            {
                earliestPostDate = user.Posts?
                    .Where(p => p != null)
                    .Select(p => (DateTime?)p.CreatedTime)
                    .Where(d => d.HasValue)
                    .OrderBy(d => d.Value)
                    .FirstOrDefault();
            }
            catch
            {
                earliestPostDate = null;
            }

            if (earliestPostDate.HasValue)
            {
                sb.AppendLine($"Approx. year joined (from first post): {earliestPostDate.Value.Year}");
            }
            else
            {
                sb.AppendLine("Approx. year joined: (not available)");
            }

            sb.AppendLine();
            sb.AppendLine("Posts by decade of age (approx.):");
            if (user.Posts == null || !user.Posts.Any())
            {
                sb.AppendLine("  No posts available.");
            }
            else if (!DateTime.TryParse(user.Birthday, out birthday))
            {
                sb.AppendLine("  Birthday not available - cannot compute age at post time.");
            }
            else
            {
                var decadeGroups = user.Posts
                    .Where(p => p != null && p.CreatedTime.HasValue)
                    .Select(p =>
                    {
                        DateTime created = p.CreatedTime.Value;
                        int ageAtPost = CalculateAge(birthday, created);
                        int decadeFloor = (ageAtPost / 10) * 10;
                        string decadeLabel = decadeFloor < 0 ? "Unknown" : $"{decadeFloor}s";
                        return new { Decade = decadeLabel };
                    })
                    .GroupBy(x => x.Decade)
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count());

                foreach (var kv in decadeGroups)
                {
                    sb.AppendLine($"  {kv.Key}: {kv.Value}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Note: analytics depend on available profile data and permissions (user_birthday, user_posts, user_photos).");

            textBoxAnalytics.Text = sb.ToString();
        }

        private int CalculateAge(DateTime birthday, DateTime atDate)
        {
            int age = atDate.Year - birthday.Year;
            if (atDate < birthday.AddYears(age))
            {
                age--;
            }
            return age;
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