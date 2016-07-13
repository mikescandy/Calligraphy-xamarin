using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Calligraphy;
using Java.Lang;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace CalligraphySampleApp
{
    [Activity(Label = "CalligraphySampleApp", MainLauncher = true, Icon = "@drawable/icon", Theme="@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Inject pragmatically
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.container, PlaceholderFragment.GetInstance())
                .Commit();


            var handler = new Handler(Looper.MainLooper);

            handler.PostDelayed(new MyRunnable(toolbar), 1000);
            handler.PostDelayed(new AnotherRunnable(toolbar), 2000);
            handler.PostDelayed(new MoreRunnable(toolbar), 3000);
        }

        /*
        Uncomment if you disable PrivateFactory injection. See CalligraphyConfig#disablePrivateFactoryInjection()
     */
        //    @Override
        //    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
        //    public View onCreateView(View parent, String name, @NonNull Context context, @NonNull AttributeSet attrs) {
        //        return CalligraphyContextWrapper.onActivityCreateView(this, parent, super.onCreateView(parent, name, context, attrs), name, context, attrs);
        //    }


        protected override void AttachBaseContext(Context newBase)
        {
            base.AttachBaseContext(CalligraphyContextWrapper.Wrap(newBase));
        }



        public class MyRunnable : Object, IRunnable
        {
            private readonly Toolbar _toolbar;

            public MyRunnable(Toolbar toolbar)
            {
                _toolbar = toolbar;
            }

            public void Run()
            {
                _toolbar.Subtitle = "Added subtitle";
            }
        }

        public class AnotherRunnable : Object, IRunnable
        {
            private readonly Toolbar _toolbar;

            public AnotherRunnable(Toolbar toolbar)
            {
                _toolbar = toolbar;
            }

            public void Run()

            {
                _toolbar.Title = null;
                _toolbar.Subtitle = "Added subtitle";
            }
        }

        public class MoreRunnable : Object, IRunnable
        {
            private readonly Toolbar _toolbar;

            public MoreRunnable(Toolbar toolbar)
            {
                _toolbar = toolbar;
            }

            public void Run()
            {
                _toolbar.Title = "Calligraphy added back";
                _toolbar.Subtitle = "Added subtitle";
            }
        }
    }
}

