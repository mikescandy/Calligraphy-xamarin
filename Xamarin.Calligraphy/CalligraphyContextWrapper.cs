using Android.App;
using Android.Content;
using Android.Util;
using Android.Views;

using Java.Lang;

namespace Calligraphy
{
    public class CalligraphyContextWrapper : ContextWrapper
    {
        private CalligraphyLayoutInflater _inflater;
        private readonly int _attributeId;

        /// <summary>
        /// Uses the default configuration from <see cref="CalligraphyConfig"/>.
        /// Remember if you are defining default in the
        /// <see cref="CalligraphyConfig"/> make sure this is initialised before
        /// the activity is created.
        /// </summary>
        /// <param name="context">base ContextBase to Wrap.</param>
        public CalligraphyContextWrapper(Context context) : base(context)
        {
            _attributeId = CalligraphyConfig.Get().AttrId;
        }

        /// <summary>
        /// Override the default AttributeId, this will always take the custom attribute defined here
        /// and ignore the one set in  <see cref="CalligraphyConfig"/>.
        /// Remember if you are defining default in the
        /// <see cref="CalligraphyConfig"/> make sure this is initialised before
        /// the activity is created.
        /// </summary>
        /// <param name="context">ContextBase to Wrap.</param>
        /// <param name="attributeId">Attribute to lookup.</param>
        public CalligraphyContextWrapper(Context context, int attributeId) : base(context)
        {
            _attributeId = attributeId;
        }

        public override Object GetSystemService(string name)
        {
            if (LayoutInflaterService.Equals(name))
            {
                return _inflater ?? (_inflater = new CalligraphyLayoutInflater(BaseContext, _attributeId));
            }
            return base.GetSystemService(name);
        }

        /**
    * Uses the default configuration from {@link uk.co.chrisjenx.calligraphy.CalligraphyConfig}
    *
    * Remember if you are defining default in the
    * {@link uk.co.chrisjenx.calligraphy.CalligraphyConfig} make sure this is initialised before
    * the activity is created.
    *
    * @param base ContextBase to Wrap.
    * @return ContextWrapper to pass back to the activity.
    */
        public static ContextWrapper Wrap(Context context)
        {
            return new CalligraphyContextWrapper(context);
        }

        /**
    * You only need to call this <b>IF</b> you call
    * {@link uk.co.chrisjenx.calligraphy.CalligraphyConfig.Builder#disablePrivateFactoryInjection()}
    * This will need to be called from the
    * {@link android.app.Activity#onCreateView(android.view.View, String, android.content.Context, android.util.AttributeSet)}
    * method to enable view font injection if the view is created inside the activity onCreateView.
    *
    * You would implement this method like so in you base activity.
    * <pre>
    * {@code
    * public View onCreateView(View parent, String name, Context context, AttributeSet attrs) {
    *   return CalligraphyContextWrapper.onActivityCreateView(this, parent, super.onCreateView(parent, name, context, attrs), name, context, attrs);
    * }
    * }
    * </pre>
    *
    * @param activity The activity the original that the ContextWrapper was attached too.
    * @param parent   Parent view from onCreateView
    * @param view     The View Created inside onCreateView or from super.onCreateView
    * @param name     The View name from onCreateView
    * @param context  The context from onCreateView
    * @param attr     The AttributeSet from onCreateView
    * @return The same view passed in, or null if null passed in.
    */
        public static View OnActivityCreateView(Activity activity, View parent, View view, string name, Context context, IAttributeSet attrs)
        {
            return Get(activity).OnActivityCreateView(parent, view, name, context, attrs);
        }

        /**
   * Get the Calligraphy Activity Fragment Instance to allow callbacks for when views are created.
   *
   * @param activity The activity the original that the ContextWrapper was attached too.
   * @return Interface allowing you to call onActivityViewCreated
   */
        static ICalligraphyActivityFactory Get(Activity activity)
        {
            if (!(activity.LayoutInflater is CalligraphyLayoutInflater))
            {
                throw new RuntimeException("This activity does not wrap the Base Context! See CalligraphyContextWrapper.wrap(Context)");
            }
            return (ICalligraphyActivityFactory)activity.LayoutInflater;
        }
    }
}