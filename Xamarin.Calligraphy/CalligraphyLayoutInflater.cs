using Android.Content;
using Android.Views;

namespace Calligraphy
{
    class CalligraphyLayoutInflater : LayoutInflater
    {
        private readonly int attributeId;

        public CalligraphyLayoutInflater(Context context, int attributeId)
            : base(context)
        {
            this.attributeId = attributeId;
            SetUpLayoutFactory();
        }

        public CalligraphyLayoutInflater(LayoutInflater original, Context newContext, int attributeId)
            : base(original, newContext)
        {

            this.attributeId = attributeId;
            SetUpLayoutFactory();
        }

        private void SetUpLayoutFactory()
        {
            if (!(Factory is CalligraphyFactory))
            {
                Factory = new CalligraphyFactory(Factory, attributeId);
            }
        }

        public override LayoutInflater CloneInContext(Context newContext)
        {
            return new CalligraphyLayoutInflater(this, newContext, attributeId);
        }
    }

}