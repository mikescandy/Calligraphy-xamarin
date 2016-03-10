using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Calligraphy;
using Java.Lang;

namespace CalligraphySampleApp
{
    [Activity(Label = "CalligraphySampleApp", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity
    {

      
    protected  override  void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        final Toolbar toolbar = findById(this, R.id.toolbar);
        setSupportActionBar(toolbar);

        // Inject pragmatically
        getSupportFragmentManager()
                .beginTransaction()
                .replace(R.id.container, PlaceholderFragment.getInstance())
                .commit();


        final Handler handler = new Handler(Looper.getMainLooper());
        handler.postDelayed(new Runnable() {
            @Override
            public void run()
    {
        //                toolbar.setTitle("Calligraphy Added");
        toolbar.setSubtitle("Added subtitle");
    }
}, 1000);

        handler.postDelayed(new Runnable()
{
    @Override public void run()
{
    toolbar.setTitle(null);
    toolbar.setSubtitle("Added subtitle");
}
        }, 2000);

        handler.postDelayed(new Runnable()
{
    @Override public void run()
{
    toolbar.setTitle("Calligraphy added back");
    toolbar.setSubtitle("Added subtitle");
}
        }, 3000);
    }

    /*
        Uncomment if you disable PrivateFactory injection. See CalligraphyConfig#disablePrivateFactoryInjection()
     */
//    @Override
//    @TargetApi(Build.VERSION_CODES.HONEYCOMB)
//    public View onCreateView(View parent, String name, @NonNull Context context, @NonNull AttributeSet attrs) {
//        return CalligraphyContextWrapper.onActivityCreateView(this, parent, super.onCreateView(parent, name, context, attrs), name, context, attrs);
//    }

 
    protected override  void AttachBaseContext(Context newBase)
{
    base.AttachBaseContext(CalligraphyContextWrapper.Wrap(newBase));
}

}

    public class MyRunnable :Object, IRunnable
    {
        public void Run()
        {
            throw new System.NotImplementedException();
        }
    }
}

