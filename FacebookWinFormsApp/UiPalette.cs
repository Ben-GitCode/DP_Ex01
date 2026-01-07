using System.Drawing;

namespace BasicFacebookFeatures
{
    // Shared UI palette instance passed from FormMain to all other forms.
    public sealed class UiPalette
    {
        // Base
        public Color FormBack { get; set; }
        public Color PanelBack { get; set; }
        public Color HeaderBack { get; set; }
        public Color PrimaryText { get; set; }
        public Color SecondaryText { get; set; }
        public Color MutedText { get; set; }

        // Lists / inputs / buttons
        public Color ListBack { get; set; }
        public Color ListFore { get; set; }
        public Color ButtonBack { get; set; }
        public Color PlaceholderText { get; set; }
        public Color PreviewImageBack { get; set; }
        public Color StatsText { get; set; }
        public Color ProfileBack { get; set; }

        // SelfAnalytics "ID card" colors
        public Color CardOuterStart { get; set; }
        public Color CardOuterEnd { get; set; }
        public Color CardInnerTop { get; set; }
        public Color CardInnerBottom { get; set; }
        public Color CardInnerBorder { get; set; }
        public Color CardShineStart { get; set; }
        public Color CardShineEnd { get; set; }
    }
}