using System.Xml;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;

namespace Calligraphy
{
    internal class CalligraphyLayoutInflater : LayoutInflater, ICalligraphyActivityFactory
    {
        private static readonly string[] ClassPrefixList = {
            "android.widget.",
            "android.webkit."
    };

        private readonly int _attributeId;
        private readonly CalligraphyFactory _calligraphyFactory;
        // Reflection Hax
        private bool _setPrivateFactory;
        private System.Reflection.FieldInfo _constructorArgs;

        public CalligraphyLayoutInflater(Context context, int attributeId) : base(context)
        {
            _attributeId = attributeId;
            _calligraphyFactory = new CalligraphyFactory(attributeId);
            SetUpLayoutFactories(false);
        }

        public CalligraphyLayoutInflater(LayoutInflater original, Context newContext, int attributeId, bool cloned) : base(original, newContext)
        {

            _attributeId = attributeId;
            _calligraphyFactory = new CalligraphyFactory(attributeId);
            SetUpLayoutFactories(cloned);
        }


        public override LayoutInflater CloneInContext(Context newContext)
        {
            return new CalligraphyLayoutInflater(this, newContext, _attributeId, true);
        }

        // ===
        // Wrapping goodies
        // ===


        public override View Inflate(XmlReader parser, ViewGroup root, bool attachToRoot)
        {
            SetPrivateFactoryInternal();
            return base.Inflate(parser, root, attachToRoot);
        }
        
        /**
         * We don't want to unnecessary create/set our factories if there are none there. We try to be
         * as lazy as possible.
         */
        private void SetUpLayoutFactories(bool cloned)
        {
            if (cloned) return;
            // If we are HC+ we get and set Factory2 otherwise we just wrap Factory1
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                if (Factory2 != null && !(Factory2.GetType() == typeof(WrapperFactory2)))
                {
                    // Sets both Factory/Factory2
                    SetFactory2(Factory2);
                }
            }
            // We can do this as setFactory2 is used for both methods.
            if (Factory != null && !(Factory.GetType() == typeof(WrapperFactory)))
            {
                SetFactory(Factory);
            }
        }



        public void SetFactory(IFactory factory)
        {
            // Only set our factory and wrap calls to the Factory trying to be set!
            if (!(Factory.GetType() == typeof(WrapperFactory)))
            {
                Factory = new WrapperFactory(factory, this, _calligraphyFactory);
            }
            else {
                Factory = factory;
            }
        }


        //@TargetApi(Build.VERSION_CODES.HONEYCOMB)
        public void SetFactory2(IFactory2 factory2)
        {
            // Only set our factory and wrap calls to the Factory2 trying to be set!
            if (!(Factory2.GetType() == typeof(WrapperFactory2)))
            {
                //            LayoutInflaterCompat.setFactory(this, new WrapperFactory2(factory2, mCalligraphyFactory));
                Factory2 = new WrapperFactory2(factory2, _calligraphyFactory);
            }
            else
            {
                Factory2 = factory2;
            }
        }

        private void SetPrivateFactoryInternal()
        {
            // Already tried to set the factory.
            if (_setPrivateFactory) return;
            // Reflection (Or Old Device) skip.
            if (!CalligraphyConfig.Get().IsReflection) return;
            // Skip if not attached to an activity.
            if (!(Context.GetType() == typeof(IFactory2)))
            {
                _setPrivateFactory = true;
                return;
            }

            var setPrivateFactoryMethod = ReflectionUtils.GetMethod(typeof(LayoutInflater), "setPrivateFactory");

            if (setPrivateFactoryMethod != null)
            {
                ReflectionUtils.InvokeMethod(this,
                        setPrivateFactoryMethod,
                        new object[] { new PrivateWrapperFactory2((IFactory2)Context, this, _calligraphyFactory) });
            }
            _setPrivateFactory = true;
        }

        // ===
        // LayoutInflater ViewCreators
        // Works in order of inflation
        // ===

        /**
         * The Activity onCreateView (PrivateFactory) is the third port of call for LayoutInflation.
         * We opted to manual injection over aggressive reflection, this should be less fragile.
         */

        //@TargetApi(Build.VERSION_CODES.HONEYCOMB)
        public View OnActivityCreateView(View parent, View view, string name, Context context, IAttributeSet attrs)
        {
            
            return _calligraphyFactory.OnViewCreated(CreateCustomViewInternal(parent, view, name, context,  attrs), context, attrs);
        }

        /**
         * The LayoutInflater onCreateView is the fourth port of call for LayoutInflation.
         * BUT only for none CustomViews.
         */

        protected override View OnCreateView(View parent, string name, IAttributeSet attrs)
        {
            var res = base.OnCreateView(parent, name, attrs);
            return _calligraphyFactory.OnViewCreated(res, Context, attrs);
        }



        /**
         * The LayoutInflater onCreateView is the fourth port of call for LayoutInflation.
         * BUT only for none CustomViews.
         * Basically if this method doesn't inflate the View nothing probably will.
         */
        //@Override

        protected override View OnCreateView(string name, IAttributeSet attrs)
        {
            // This mimics the {@code PhoneLayoutInflater} in the way it tries to inflate the base
            // classes, if this fails its pretty certain the app will fail at this point.
            View view = null;
            foreach (var prefix in ClassPrefixList)
            {
                try
                {
                    view = CreateView(name, prefix, attrs);
                }
                catch (System.Exception ignored)
                {
                }
            }
            // In this case we want to let the base class take a crack
            // at it.
            if (view == null) view = base.OnCreateView(name, attrs);

            return _calligraphyFactory.OnViewCreated(view, view.Context, attrs);
        }


        /**
         * Nasty method to inflate custom layouts that haven't been handled else where. If this fails it
         * will fall back through to the PhoneLayoutInflater method of inflating custom views where
         * Calligraphy will NOT have a hook into.
         *
         * @param parent      parent view
         * @param view        view if it has been inflated by this point, if this is not null this method
         *                    just returns this value.
         * @param name        name of the thing to inflate.
         * @param viewContext Context to inflate by if parent is null
         * @param attrs       Attr for this view which we can steal fontPath from too.
         * @return view or the View we inflate in here.
         */

        internal View CreateCustomViewInternal(View parent, View view, string name, Context viewContext, IAttributeSet attrs)
        {
            // I by no means advise anyone to do this normally, but Google have locked down access to
            // the createView() method, so we never get a callback with attributes at the end of the
            // createViewFromTag chain (which would solve all this unnecessary rubbish).
            // We at the very least try to optimise this as much as possible.
            // We only call for customViews (As they are the ones that never go through onCreateView(...)).
            // We also maintain the Field reference and make it accessible which will make a pretty
            // significant difference to performance on Android 4.0+.

            // If CustomViewCreation is off skip this.
            if (!CalligraphyConfig.Get().IsCustomViewCreation) return view;
            if (view == null && name.IndexOf('.') > -1)
            {
                if (_constructorArgs == null)
                    _constructorArgs = ReflectionUtils.GetFieldInfo(typeof(LayoutInflater), "mConstructorArgs");

                var mConstructorArgsArr = (object[])ReflectionUtils.GetValue(_constructorArgs, this);
                var lastContext = mConstructorArgsArr[0];
                // The LayoutInflater actually finds out the correct context to use. We just need to set
                // it on the mConstructor for the internal method.
                // Set the constructor ars up for the createView, not sure why we can't pass these in.
                mConstructorArgsArr[0] = viewContext;
                ReflectionUtils.SetValue(_constructorArgs, this, mConstructorArgsArr);
                try
                {
                    view = CreateView(name, null, attrs);
                }
                catch (System.Exception ignored)
                {
                }
                finally
                {
                    mConstructorArgsArr[0] = lastContext;
                    ReflectionUtils.SetValue(_constructorArgs, this, mConstructorArgsArr);
                }
            }
            return view;
        }

        

        /**
         * Factory 2 is the second port of call for LayoutInflation
         */
        //@TargetApi(Build.VERSION_CODES.HONEYCOMB)
        private class WrapperFactory2 : Java.Lang.Object, IFactory2
        {
            protected IFactory2 MFactory2;
            protected CalligraphyFactory MCalligraphyFactory;

            public WrapperFactory2(IFactory2 factory2, CalligraphyFactory calligraphyFactory)
            {
                MFactory2 = factory2;
                MCalligraphyFactory = calligraphyFactory;
            }

            //@Override
            public View OnCreateView(string name, Context context, IAttributeSet attrs)
            {
                return MCalligraphyFactory.OnViewCreated(MFactory2.OnCreateView(name, context, attrs), context, attrs);
            }

            //@Override
            public virtual View OnCreateView(View parent, string name, Context context, IAttributeSet attrs)
            {
                return MCalligraphyFactory.OnViewCreated(MFactory2.OnCreateView(parent, name, context, attrs), context, attrs);
            }
        }

        /**
         * Private factory is step three for Activity Inflation, this is what is attached to the
         * Activity on HC+ devices.
         */
        //@TargetApi(Build.VERSION_CODES.HONEYCOMB)
        private class PrivateWrapperFactory2 : WrapperFactory2
        {

            private CalligraphyLayoutInflater _mInflater;

            public PrivateWrapperFactory2(IFactory2 factory2, CalligraphyLayoutInflater inflater, CalligraphyFactory calligraphyFactory) : base(factory2, calligraphyFactory)
            {

                _mInflater = inflater;
            }

            //@Override
            public override View OnCreateView(View parent, string name, Context context, IAttributeSet attrs)
            {
                return MCalligraphyFactory.OnViewCreated(
                        _mInflater.CreateCustomViewInternal(parent,
                                MFactory2.OnCreateView(parent, name, context, attrs),
                                name, context, attrs
                        ),
                        context, attrs
                );
            }
        }
    }

}