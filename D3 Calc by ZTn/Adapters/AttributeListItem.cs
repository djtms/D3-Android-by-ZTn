using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZTn.BNet.D3.Heroes;
using ZTn.BNet.D3.Items;

namespace ZTnDroid.D3Calculator.Adapters
{
    public class AttributeListItem : IListItem
    {
        public String name;
        public String value;

        protected AttributeListItem()
        {
        }

        public AttributeListItem(String name, String value)
        {
            this.name = name;
            this.value = value;
        }

        public AttributeListItem(String name, HeroClass value, Context context)
        {
            this.name = name;
            switch (value)
            {
                case HeroClass.Barbarian:
                    this.value = context.Resources.GetString(Resource.String.barbarian);
                    break;
                case HeroClass.DemonHunter:
                    this.value = context.Resources.GetString(Resource.String.demonHunter);
                    break;
                case HeroClass.Monk:
                    this.value = context.Resources.GetString(Resource.String.monk);
                    break;
                case HeroClass.WitchDoctor:
                    this.value = context.Resources.GetString(Resource.String.witchDoctor);
                    break;
                case HeroClass.Wizard:
                    this.value = context.Resources.GetString(Resource.String.wizard);
                    break;
                default:
                    this.value = "Unknown";
                    break;
            }
        }

        public AttributeListItem(String name, DateTime value)
        {
            this.name = name;
            this.value = value.ToString();
        }

        public AttributeListItem(String name, ItemValueRange value)
        {
            this.name = name;
            this.value = value.min.ToString();
        }

        public AttributeListItem(String name, long value)
        {
            this.name = name;
            this.value = value.ToString();
        }

        public AttributeListItem(String name, double value)
        {
            this.name = name;
            this.value = String.Format("{0:0.00}", value);
        }

        public int getLayoutResource()
        {
            return Resource.Layout.AttributeListItem;
        }

        public void updateHeroView(View view)
        {
            view.FindViewById<TextView>(Resource.Id.attributeName).Text = name;
            view.FindViewById<TextView>(Resource.Id.attributeValue).Text = value;
        }
    }
}