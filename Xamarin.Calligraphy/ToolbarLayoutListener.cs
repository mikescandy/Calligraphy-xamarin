using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Calligraphy
{
    public class ToolbarLayoutListener : Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private const string Blank = " ";

        private readonly WeakReference<CalligraphyFactory> _calligraphyFactory;
        private readonly WeakReference<Context> _contextRef;
        private readonly WeakReference<Toolbar> _toolbarReference;
        private readonly string _originalSubTitle;

        public ToolbarLayoutListener(CalligraphyFactory calligraphyFactory,
                                   Context context, Toolbar toolbar)
        {
            _calligraphyFactory = new WeakReference<CalligraphyFactory>(calligraphyFactory);
            _contextRef = new WeakReference<Context>(context);
            _toolbarReference = new WeakReference<Toolbar>(toolbar);
            _originalSubTitle = toolbar.Subtitle;
            toolbar.Subtitle = Blank;
        }

        //   @TargetApi(Build.VERSION_CODES.JELLY_BEAN)
        public void OnGlobalLayout()
        {
            Toolbar toolbar;
            _toolbarReference.TryGetTarget(out toolbar);
            Context context;
            _contextRef.TryGetTarget(out context);

            CalligraphyFactory factory;
            _calligraphyFactory.TryGetTarget(out factory);
            if (toolbar == null) return;
            if (factory == null || context == null)
            {
                RemoveSelf(toolbar);
                return;
            }

            var childCount = toolbar.ChildCount;
            if (childCount != 0)
            {
                // Process children, defer draw as it has set the typeface.
                for (int i = 0; i < childCount; i++)
                {
                    factory.OnViewCreated(toolbar.GetChildAt(i), context, null);
                }
            }
            RemoveSelf(toolbar);
            toolbar.Subtitle = _originalSubTitle;
        }

        private void RemoveSelf(Toolbar toolbar)
        {// Our dark deed is done
            if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBean)
            {
                //noinspection deprecation
#pragma warning disable 618
                toolbar.ViewTreeObserver.RemoveGlobalOnLayoutListener(this);
#pragma warning restore 618
            }
            else {
                toolbar.ViewTreeObserver.RemoveOnGlobalLayoutListener(this);
            }
        }

    }

}