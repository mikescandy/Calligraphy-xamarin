using System;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace Calligraphy
{
    internal class CalligraphyTypefaceSpan : MetricAffectingSpan
    {
        private readonly Typeface _typeface;

        public override void UpdateDrawState(TextPaint drawState)
        {
            Apply(drawState);
        }

        public override void UpdateMeasureState(TextPaint measureState)
        {
            Apply(measureState);
        }

        internal CalligraphyTypefaceSpan(Typeface typeface)
        {
            if (typeface == null)
            {
                throw new ArgumentNullException(nameof(typeface));
            }
            _typeface = typeface;
        }

        private void Apply(Paint paint)
        {
            var oldTypeface = paint.Typeface;
            var oldStyle = oldTypeface?.Style ?? 0;
            var fakeStyle = oldStyle & _typeface.Style;

            if ((fakeStyle & TypefaceStyle.Bold) != 0)
            {
                paint.FakeBoldText = true;
            }

            if ((fakeStyle & TypefaceStyle.Italic) != 0)
            {
                paint.TextSkewX = -0.25f;
            }

            paint.SetTypeface(_typeface);
        }
    }
}