using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = System.Math;
using String = System.String;

namespace CalligraphySampleApp
{
    [Register("calligraphysampleapp.CustomViewWithTypefaceSupport")]
    public class CustomViewWithTypefaceSupport : View
    {

    private Paint paint;
    private Rect textBounds;
    private int width;
    private int height;

    public CustomViewWithTypefaceSupport(Context context):base(context)
    {
        
        Init();
    }

    public CustomViewWithTypefaceSupport(Context context, IAttributeSet attrs):base(context,attrs)
    {
        Init();
    }

    public CustomViewWithTypefaceSupport(Context context, IAttributeSet attrs, int defStyleAttr):base(context,attrs,defStyleAttr)
    {
        Init();
    }

//    @TargetApi(Build.VERSION_CODES.LOLLIPOP)
    public CustomViewWithTypefaceSupport(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes):base(context, attrs, defStyleAttr, defStyleRes)
        {
        Init();
    }

    private void Init()
    {
        paint = new Paint {TextSize = 50};
        textBounds = new Rect();
    }

   // @Override
    protected override void OnDraw(Canvas canvas)
    {
        var text = "This is a custom view with setTypeface support";
        var fm = paint.GetFontMetrics();
        paint.GetTextBounds(text, 0, text.Length, textBounds);

        width = textBounds.Left + textBounds.Right + PaddingLeft + PaddingRight;
        height = (int)(Math.Abs(fm.Top) + fm.Bottom);

        canvas.DrawText(text, 0, -fm.Top + PaddingTop, paint);
    }

    //@Override
    protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
    {
        SetMeasuredDimension(width, height);
    }

    /**
     * Used by Calligraphy to change view's typeface
     */
    //@SuppressWarnings("unused")
    public void SetTypeface(Typeface tf)
    {
        paint.SetTypeface(tf);
        Invalidate();
    }
}

}