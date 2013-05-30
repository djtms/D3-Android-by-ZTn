using System.Reflection;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using ZTn.BNet.D3.DataProviders;

using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;

using Fragment = Android.Support.V4.App.Fragment;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class SettingsFragment : Fragment
    {
        #region >> Fragment

        /// <inheritdoc/>
        public override void OnCreate(Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());
            base.OnCreate(savedInstanceState);
        }

        /// <inheritdoc/>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());
            View view = inflater.Inflate(Resource.Layout.Settings, container, false);

            Activity.Title = Resources.GetString(Resource.String.Settings);

            Switch settingOnline = view.FindViewById<Switch>(Resource.Id.settingOnline);
            settingOnline.Checked = (D3Context.instance.onlineMode == OnlineMode.Online);
            settingOnline.CheckedChange += delegate(object sender, CompoundButton.CheckedChangeEventArgs e)
            {
                D3Calc.preferences
                    .Edit()
                    .PutBoolean(D3Calc.SETTINGS_ONLINEMODE, e.IsChecked)
                    .Commit();
                D3Context.instance.onlineMode = (e.IsChecked ? OnlineMode.Online : OnlineMode.Offline);
            };

            return view;
        }

        #endregion
    }
}