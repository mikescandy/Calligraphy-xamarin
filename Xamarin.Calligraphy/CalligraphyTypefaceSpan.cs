using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace Calligraphy
{
    class CalligraphyTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface typeface;

        internal CalligraphyTypefaceSpan(Typeface typeface)
        {
            this.typeface = typeface;
        }

        private void Apply(Paint paint)
        {
            var oldTypeface = paint.Typeface;
            var oldStyle = oldTypeface != null ? oldTypeface.Style : 0;
            var fakeStyle = oldStyle & typeface.Style;

            if ((fakeStyle & TypefaceStyle.Bold) != 0)
            {
                paint.FakeBoldText = true;
            }

            if ((fakeStyle & TypefaceStyle.Italic) != 0)
            {
                paint.TextSkewX = -0.25f;
            }

            paint.SetTypeface(typeface);
        }

        public override void UpdateDrawState(TextPaint drawState)
        {
            Apply(drawState);
        }

        public override void UpdateMeasureState(TextPaint measureState)
        {
            Apply(measureState);
        }
    }
}