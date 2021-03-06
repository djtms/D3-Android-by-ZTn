using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using ZTn.BNet.BattleNet;
using ZTn.BNet.D3;
using ZTn.BNet.D3.Careers;
using ZTn.BNet.D3.DataProviders;
using ZTnDroid.D3Calculator.Adapters;
using ZTnDroid.D3Calculator.Adapters.Delegated;
using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;
using Debug = System.Diagnostics.Debug;
using Environment = System.Environment;
using Fragment = Android.Support.V4.App.Fragment;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class HeroesListFragment : Fragment
    {
        private String battleTag;
        private String host;

        private Career career;

        #region >> Fragment

        /// <inheritdoc/>
        public override void OnCreate(Bundle savedInstanceState)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            base.OnCreate(savedInstanceState);

            RetainInstance = true;

            HasOptionsMenu = true;

            battleTag = D3Context.Instance.BattleTag;
            host = D3Context.Instance.Host;
        }

        /// <inheritdoc/>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            var view = inflater.Inflate(Resource.Layout.ViewCareer, container, false);

            var listView = view.FindViewById<ListView>(Resource.Id.HeroesListView);
            listView.ItemClick += (sender, args) =>
            {
                var heroSummary = ((HeroSummariesListAdapter)listView.Adapter).GetHeroSummaryAt(args.Position);
                D3Context.Instance.CurrentHeroSummary = heroSummary;

                var viewHeroIntent = new Intent(Activity, typeof(ViewHeroActivity));

                StartActivity(viewHeroIntent);
            };

            Activity.Title = battleTag;

            DeferredFetchAndUpdateCareer(D3Context.Instance.FetchMode);

            return view;
        }

        /// <inheritdoc/>
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            base.OnCreateOptionsMenu(menu, inflater);

            Activity.MenuInflater.Inflate(Resource.Menu.ViewCareerActivity, menu);
        }

        /// <inheritdoc/>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            switch (item.ItemId)
            {
                case Resource.Id.DeleteContent:
                    DeleteCareer();
                    return true;

                case Resource.Id.RefreshContent:
                    DeferredFetchAndUpdateCareer(FetchMode.Online);
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        #endregion

        private void DeferredFetchAndUpdateCareer(FetchMode online)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            ProgressDialog progressDialog = null;

            if (online == FetchMode.Online)
            {
                progressDialog = ProgressDialog.Show(Activity, Resources.GetString(Resource.String.LoadingCareer), Resources.GetString(Resource.String.WaitWhileRetrievingData), true);
            }

            new Thread(() =>
            {
                try
                {
                    FetchCareer(online);
                    Activity.RunOnUiThread(() =>
                    {
                        if (online == FetchMode.Online)
                        {
                            Debug.Assert(progressDialog != null, "progressDialog != null");
                            progressDialog.Dismiss();
                        }
                        UpdateCareerView();
                    });
                }
                catch (FileNotInCacheException)
                {
                    if (Activity != null)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            if (online == FetchMode.Online)
                            {
                                progressDialog.Dismiss();
                            }
                            Toast.MakeText(Activity, "Career not in cache" + Environment.NewLine + "Please use refresh action", ToastLength.Long)
                                .Show();
                        });
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("An exception occured in activity {0}: {1}", Activity, exception);
                    if (Activity != null)
                    {
                        Activity.RunOnUiThread(() =>
                        {
                            if (online == FetchMode.Online)
                            {
                                progressDialog.Dismiss();
                            }
                            Toast.MakeText(Activity, Resources.GetString(Resource.String.ErrorOccuredWhileRetrievingData), ToastLength.Long)
                                .Show();
                        });
                    }
                }
            }).Start();
        }

        private void DeleteCareer()
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            Toast.MakeText(Activity, "Career will be removed...", ToastLength.Short);
            D3Context.Instance.DbAccounts.Delete(battleTag, host);
            Activity.Finish();
        }

        private void FetchCareer(FetchMode online)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            D3Api.Host = host;
            var dataProvider = (CacheableDataProvider)D3Api.DataProvider;
            dataProvider.FetchMode = online;

            try
            {
                career = Career.CreateFromBattleTag(new BattleTag(battleTag));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                career = null;
                throw;
            }
            finally
            {
                dataProvider.FetchMode = D3Context.Instance.FetchMode;
            }
        }

        private void UpdateCareerView()
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            if (career == null)
            {
                var messageView = Activity.FindViewById<TextView>(Resource.Id.messageLabel);
                messageView.Text = Resources.GetString(Resource.String.ErrorOccuredWhileRetrievingData);
                messageView.Visibility = ViewStates.Visible;
                var heroesLabel = Activity.FindViewById<TextView>(Resource.Id.sectionLabel);
                heroesLabel.Visibility = ViewStates.Gone;
                return;
            }

            var killsListView = Activity.FindViewById<ListView>(Resource.Id.killsLifetimeListView);
            var attributes = new List<IListItem>
            {
                new SectionHeaderListItem(Resources.GetString(Resource.String.KillsLifetime)),
                new AttributeListItem(Resources.GetString(Resource.String.elites), career.Kills.Elites),
                new AttributeListItem(Resources.GetString(Resource.String.monsters), career.Kills.Monsters),
                new AttributeListItem(Resources.GetString(Resource.String.monstersHardcore), career.Kills.HardcoreMonsters),
                new AttributeListItem(Resources.GetString(Resource.String.paragon), career.ParagonLevel),
                new AttributeListItem(Resources.GetString(Resource.String.paragonHardcore), career.ParagonLevelHardcore)
            };
            killsListView.Adapter = new ListAdapter(Activity, attributes.ToArray());

            if (career.Heroes != null)
            {
                var listView = Activity.FindViewById<ListView>(Resource.Id.HeroesListView);
                listView.Adapter = new HeroSummariesListAdapter(Activity, career.Heroes);
            }
        }
    }
}