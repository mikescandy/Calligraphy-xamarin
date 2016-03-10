using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;

namespace Calligraphy
{
    // ===
    // Wrapper Factories for Pre/Post HC
    // ===

    /**
     * Factory 1 is the first port of call for LayoutInflation
     */
    internal class WrapperFactory : Java.Lang.Object, LayoutInflater.IFactory
    {

        private readonly LayoutInflater.IFactory _factory;
        private readonly CalligraphyLayoutInflater _inflater;
        private CalligraphyFactory _calligraphyFactory;

        public WrapperFactory(LayoutInflater.IFactory factory, CalligraphyLayoutInflater inflater, CalligraphyFactory calligraphyFactory)
        {
            _factory = factory;
            _inflater = inflater;
            _calligraphyFactory = calligraphyFactory;
        }

        public View OnCreateView(string name, Context context, IAttributeSet attrs)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Honeycomb)
            {
                return _calligraphyFactory.OnViewCreated(
                        _inflater.CreateCustomViewInternal(
                                null, _factory.OnCreateView(name, context, attrs), name, context, attrs
                        ),
                        context, attrs
                );
            }
            return _calligraphyFactory.OnViewCreated(
                    _factory.OnCreateView(name, context, attrs),
                    context, attrs
            );
        }
    }
}