using Android.Graphics;
using Android.Text;
using Java.Lang;

namespace Calligraphy
{
    internal class TextWatcher : Object, ITextWatcher
    {
        private readonly Typeface _typeface;

        internal TextWatcher(Typeface typeface)
        {
            _typeface = typeface;
        }

        public void AfterTextChanged(IEditable s)
        {
            CalligraphyUtils.ApplyTypefaceSpan(s, _typeface);
        }

        public void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
        }
    }
}