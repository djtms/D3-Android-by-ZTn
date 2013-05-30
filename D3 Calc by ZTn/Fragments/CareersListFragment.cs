using System;
using System.Reflection;

using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Views;
using Android.Widget;

using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;

using Fragment = Android.Support.V4.App.Fragment;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class CareersListFragment : Fragment
    {
        const int ADD_NEW_ACCOUNT = 0;

        readonly String[] accountsFromColumns = new String[] { Storage.AccountsOpenHelper.FIELD_BATTLETAG, Storage.AccountsOpenHelper.FIELD_HOST };
        readonly int[] accountsToId = new int[] { Android.Resource.Id.Text1, Android.Resource.Id.Text2 };

        ICursor cursor;

        #region >> Fragment

        /// <inheritdoc/>
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            switch (requestCode)
            {
                case ADD_NEW_ACCOUNT:
                    switch (resultCode)
                    {
                        case -1:
                            String battleTag = data.GetStringExtra("battleTag");
                            String host = data.GetStringExtra("host");

                            D3Context.instance.dbAccounts.insert(battleTag, host);

                            IListAdapter careerAdapter = new SimpleCursorAdapter(Activity, Android.Resource.Layout.SimpleListItem2, cursor, accountsFromColumns, accountsToId);
                            Activity.FindViewById<ListView>(Resource.Id.AccountsListView).Adapter = careerAdapter;

                            Toast.MakeText(Activity, "Account added", ToastLength.Short).Show();
                            break;

                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <inheritdoc/>
        public override void OnCreate(Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            base.OnCreate(savedInstanceState);

            RetainInstance = true;

            SetHasOptionsMenu(true);
        }

        /// <inheritdoc/>
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Activity.MenuInflater.Inflate(Resource.Menu.HomeActivity, menu);

            base.OnCreateOptionsMenu(menu, inflater);
        }

        /// <inheritdoc/>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            View view = inflater.Inflate(Resource.Layout.Home, container, false);

            ListView careerListView = view.FindViewById<ListView>(Resource.Id.AccountsListView);
            careerListView.ItemClick += (Object sender, AdapterView.ItemClickEventArgs args) =>
            {
                Intent viewCareerIntent = new Intent(Activity, typeof(ViewCareerActivity));
                D3Context.instance.battleTag = args.View.FindViewById<TextView>(Android.Resource.Id.Text1).Text;
                D3Context.instance.host = args.View.FindViewById<TextView>(Android.Resource.Id.Text2).Text;
                StartActivity(viewCareerIntent);
            };

            D3Context.instance.dbAccounts = new AccountsDB(Activity);
            cursor = D3Context.instance.dbAccounts.getAccounts();
            Activity.StartManagingCursor(cursor);

            IListAdapter accountsAdapter = new SimpleCursorAdapter(Activity, Android.Resource.Layout.SimpleListItem2, cursor, accountsFromColumns, accountsToId);
            view.FindViewById<ListView>(Resource.Id.AccountsListView).Adapter = accountsAdapter;

            return view;
        }

        /// <inheritdoc/>
        public override void OnDestroyView()
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Activity.StopManagingCursor(cursor);
            cursor.Close();

            base.OnDestroyView();
        }

        /// <inheritdoc/>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            switch (item.ItemId)
            {
                case Resource.Id.AddNewAccount:
                    Intent intent = new Intent(Activity, typeof(AddNewAccountActivity));
                    StartActivityForResult(intent, ADD_NEW_ACCOUNT);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        #endregion

        private void insertIntoCareersStorage(String battleTag, String host)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            D3Context.instance.dbAccounts.insert(battleTag, host);
        }
    }
}