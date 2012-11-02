﻿using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZTn.BNet.BattleNet;
using ZTn.BNet.D3;
using ZTn.BNet.D3.Careers;
using ZTn.BNet.D3.Heroes;
using ZTnDroid.D3Calculator.Fragments;
using ZTnDroid.D3Calculator.Storage;

namespace ZTnDroid.D3Calculator
{
    [Activity(Label = "D3 Calc by ZTn", MainLauncher = true, Theme = "@android:style/Theme.Holo", Icon = "@drawable/icon")]
    public class HomeActivity : Activity
    {
        public override void OnBackPressed()
        {
            Finish();
            base.OnBackPressed();
        }

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("HomeActivity: OnCreate");
            base.OnCreate(bundle);

            this.Application.SetTheme(Android.Resource.Style.ThemeHolo);

            SetContentView(Resource.Layout.FragmentContainer);

            Fragment fragment = new CareersListFragment();
            FragmentTransaction fragmentTransaction = FragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.fragment_container, fragment);
            fragmentTransaction.Commit();

            // Always start D3Api with cache available and offline
            DataProviders.CacheableDataProvider dataProvider = new DataProviders.CacheableDataProvider(this, new ZTn.BNet.D3.DataProviders.HttpRequestDataProvider());
            dataProvider.online = false;
            D3Api.dataProvider = dataProvider;
        }
    }
}

