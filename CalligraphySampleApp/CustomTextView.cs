using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace CalligraphySampleApp
{
    public class CustomTextView :TextView
    {

    public CustomTextView(Context context):base(context)
    {
    }

    public CustomTextView(Context context, IAttributeSet attrs):base(context,attrs)
    {
    }

    public CustomTextView(Context context, IAttributeSet attrs, int defStyle):base(context,attrs,defStyle)
    {
    }

}
}