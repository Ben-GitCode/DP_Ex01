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

        // Runtime constructor that receives login state
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

            // Basic info
            sb.AppendLine($"Name: {user.Name}");
            sb.AppendLine();

            // Birthday and age
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

            // Age range (official)
            // The FacebookWrapper.User type used in this project does not expose an AgeRange property.
            sb.AppendLine("Official age range: (not available)");

            // Gender
            sb.AppendLine($"Gender: {user.Gender?.ToString() ?? "(not available)"}");

            sb.AppendLine();

            // Counts
            int albumsCount = user.Albums?.Count() ?? 0;
            int postsCount = user.Posts?.Count() ?? 0;
            int photosCount = user.PhotosTaggedIn?.Count() ?? 0;
            sb.AppendLine($"Albums: {albumsCount}");
            sb.AppendLine($"Posts: {postsCount}");
            sb.AppendLine($"Tagged Photos: {photosCount}");
            sb.AppendLine();

            // Year joined Facebook - use earliest post creation if available
            DateTime? earliestPostDate = null;
            try
            {
                earliestPostDate = user.Posts?
                    .Where(p => p != null)
                    .Select(p => p.CreatedTime)
                    .Where(d => d.HasValue)
                    .OrderBy(d => d.Value)
                    .FirstOrDefault();
            }
            catch
            {
                // ignore if CreatedTime isn't accessible
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

            // Posts per decade of age
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

            // Write to UI
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
    }
}