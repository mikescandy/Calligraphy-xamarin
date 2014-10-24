using Android.Content;
using Android.Views;

using Java.Lang;

namespace Calligraphy
{
    public class CalligraphyContextWrapper : ContextWrapper
    {
        private LayoutInflater inflater;
        private readonly int attributeId;

        /// <summary>
        /// Uses the default configuration from <see cref="CalligraphyConfig"/>.
        /// Remember if you are defining default in the
        /// <see cref="CalligraphyConfig"/> make sure this is initialised before
        /// the activity is created.
        /// </summary>
        /// <param name="context">base ContextBase to Wrap.</param>
        public CalligraphyContextWrapper(Context context)
            : base(context)
        {
            attributeId = CalligraphyConfig.Get().AttrId;
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
        public CalligraphyContextWrapper(Context context, int attributeId)
            : base(context)
        {
            this.attributeId = attributeId;
        }

        public override Object GetSystemService(string name)
        {
            if (LayoutInflaterService.Equals(name))
            {
                return inflater ?? (inflater = new CalligraphyLayoutInflater(LayoutInflater.From(BaseContext), this, attributeId));
            }
            return base.GetSystemService(name);
        }
    }
}