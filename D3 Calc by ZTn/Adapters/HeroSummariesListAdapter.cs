using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using ZTn.BNet.D3.Heroes;
using ZTnDroid.D3Calculator.Helpers;

namespace ZTnDroid.D3Calculator.Adapters
{
    public class HeroSummariesListAdapter : BaseAdapter
    {
        readonly Context context;
        readonly HeroSummary[] heroes;

        public HeroSummariesListAdapter(Context context, HeroSummary[] heroes)
        {
            this.context = context;
            this.heroes = heroes;
        }

        public override int Count
        {
            get { return heroes.Length; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? ((Activity)context).LayoutInflater.Inflate(Resource.Layout.HeroesListItem, parent, false);

            var hero = heroes[position];
            view.FindViewById<TextView>(Resource.Id.heroName).Text = heroes[position].Name;
            view.FindViewById<TextView>(Resource.Id.heroClass).Text = hero.HeroClass.Translate().CapitalizeFirstLetter();
            view.FindViewById<TextView>(Resource.Id.heroLevel).Text = String.Format("{0}", hero.Level);
            view.FindViewById<TextView>(Resource.Id.heroParagon).Text = String.Format("+{0}", hero.ParagonLevel);
            view.FindViewById<TextView>(Resource.Id.heroHardcore).Text = (hero.Hardcore ? "hardcore" : "");
            view.FindViewById<TextView>(Resource.Id.heroSeasonal).Text = (hero.Seasonal ? "season" : "");
            view.FindViewById<TextView>(Resource.Id.heroLastUpdated).Text = hero.LastUpdated.ToString("dd/MM/yyyy HH:mm");

            var imageResource = hero.HeroClass.GetPortraitResource(hero.Gender);
            view.FindViewById<ImageView>(Resource.Id.imageClass).SetImageResource(imageResource);

            return view;
        }

        public HeroSummary GetHeroSummaryAt(int position)
        {
            return heroes[position];
        }
    }
}