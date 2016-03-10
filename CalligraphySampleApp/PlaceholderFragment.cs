using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Com.Lilarcor.Cheeseknife;

namespace CalligraphySampleApp
{
    public class PlaceholderFragment : Fragment
    {
        public static Fragment GetInstance()
        {
            return new PlaceholderFragment();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle state)
        {
            return inflater.Inflate(Resource.Layout.fragment_main, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            Cheeseknife.Inject(this, view);

            var stub = view.FindViewById<ViewStub>(Resource.Id.stub);
            Cheeseknife.Inject(view, stub);

            var stubWithFontPath = view.FindViewById<ViewStub>(Resource.Id.stub_with_font_path);
            Cheeseknife.Inject(view, stubWithFontPath);

        }

        [InjectOnClick(Resource.Id.button_bold)]
        public void OnClickBoldButton()
        {
            Toast.MakeText(Activity, "Custom Typeface toast text", ToastLength.Short).Show();
        }

        [InjectOnClick(Resource.Id.button_default)]
        public void OnClickDefaultButton()
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetMessage("Custom Typeface Dialog");
            builder.SetTitle("Sample Dialog");
            builder.SetPositiveButton("OK", (sender, args) => ((Dialog) sender).Dismiss());
            builder.Create().Show();
        }
    }
}