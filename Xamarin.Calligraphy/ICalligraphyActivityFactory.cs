using System;
using Android.Views;
using Android.Content;

namespace Calligraphy
{
	public interface ICalligraphyActivityFactory
	{
		/// <summary>
		/// Used to Wrap the Activity onCreateView method.
		/// You implement this method like so in you base activity.
		/// <pre>
		/// {@code
		///	public View onCreateView(View parent, String name, Context context, AttributeSet attrs) {
		///	  return CalligraphyContextWrapper.get(getBaseContext()).onActivityCreateView(super.onCreateView(parent, name, context, attrs), attrs);
		///	}
		///	}
		/// </pre>
		/// </summary>
		/// <returns>The view passed in, or null if nothing was passed in..</returns>
		/// <param name="parent">parent view, can be null.</param>
		/// <param name="view">result of {@code super.onCreateView(parent, name, context, attrs)}, this might be null, which is fine..</param>
		/// <param name="name">Name of View we are trying to inflate.</param>
		/// <param name="context">current context (normally the Activity's).</param>
		/// <param name="attrs">see {@link android.view.LayoutInflater.Factory2#onCreateView(android.view.View, String, android.content.Context, android.util.AttributeSet)}  @return the result from the activities {@code onCreateView()}.</param>
		View OnActivityCreateView(View parent, View view, string name, Context context, object[] attrs);
	}
}

