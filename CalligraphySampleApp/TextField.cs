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
    public class TextField : TextView
    {

    public TextField(Context context, IAttributeSet attrs):base(context,attrs, Resource.Attribute.textFieldStyle)
    {
    }

}
}