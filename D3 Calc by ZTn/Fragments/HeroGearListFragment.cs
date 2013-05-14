using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ZTn.BNet.D3.Heroes;
using ZTn.BNet.D3.Items;
using ZTn.BNet.D3.Medias;
using ZTn.BNet.D3.Calculator.Sets;
using ZTn.BNet.D3.Calculator.Helpers;
using ZTnDroid.D3Calculator.Adapters;
using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;
using Fragment = Android.Support.V4.App.Fragment;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class HeroGearListFragment : ZTnFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            base.OnCreate(savedInstanceState);

            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            View view = inflater.Inflate(Resource.Layout.ViewHeroGear, container, false);

            updateView(view);

            return view;
        }

        private List<IListItem> getDataForItem(int id, ItemSummary item, D3Picture icon)
        {
            return getDataForItem(Resources.GetString(id), item, icon);
        }

        private List<IListItem> getDataForItem(String label, ItemSummary item, D3Picture icon)
        {
            List<IListItem> list = new List<IListItem>() {
                new SectionHeaderListItem(label)
            };

            if (item != null && (item is Item) && (icon != null))
                list.Add(new GearItemListItem((Item)item, icon));
            else if (item != null && (item is Item))
                list.Add(new GearItemListItem((Item)item));

            return list;
        }

        private void updateView(View view)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Hero hero = D3Context.instance.hero;
            IconsContainer icons = D3Context.instance.icons;
            if (hero != null && hero.items != null)
            {
                ListView heroGearListView = view.FindViewById<ListView>(Resource.Id.heroGearListView);
                List<IListItem> gearAttr = new List<IListItem>();

                gearAttr.AddRange(getDataForItem(Resource.String.itemHead, hero.items.head, icons.head));
                gearAttr.AddRange(getDataForItem(Resource.String.itemTorso, hero.items.torso, icons.torso));
                gearAttr.AddRange(getDataForItem(Resource.String.itemFeet, hero.items.feet, icons.feet));
                gearAttr.AddRange(getDataForItem(Resource.String.itemHands, hero.items.hands, icons.hands));
                gearAttr.AddRange(getDataForItem(Resource.String.itemShoulders, hero.items.shoulders, icons.shoulders));
                gearAttr.AddRange(getDataForItem(Resource.String.itemLegs, hero.items.legs, icons.legs));
                gearAttr.AddRange(getDataForItem(Resource.String.itemBracers, hero.items.bracers, icons.bracers));
                gearAttr.AddRange(getDataForItem(Resource.String.itemMainHand, hero.items.mainHand, icons.mainHand));
                gearAttr.AddRange(getDataForItem(Resource.String.itemOffHand, hero.items.offHand, icons.offHand));
                gearAttr.AddRange(getDataForItem(Resource.String.itemWaist, hero.items.waist, icons.waist));
                gearAttr.AddRange(getDataForItem(Resource.String.itemRightFinger, hero.items.rightFinger, icons.rightFinger));
                gearAttr.AddRange(getDataForItem(Resource.String.itemLeftFinger, hero.items.leftFinger, icons.leftFinger));
                gearAttr.AddRange(getDataForItem(Resource.String.itemNeck, hero.items.neck, icons.neck));

                HeroItems heroItems = D3Context.instance.hero.items;
                List<Item> items = new List<Item>() {
                    (Item)heroItems.bracers,
                    (Item)heroItems.feet,
                    (Item)heroItems.hands,
                    (Item)heroItems.head,
                    (Item)heroItems.leftFinger,
                    (Item)heroItems.legs,
                    (Item)heroItems.neck,
                    (Item)heroItems.rightFinger,
                    (Item)heroItems.shoulders,
                    (Item)heroItems.torso,
                    (Item)heroItems.waist,
                    (Item)heroItems.mainHand,
                    (Item)heroItems.offHand
                };
                items = items.Where(i => i != null).ToList();

                foreach (Set set in D3Context.instance.activatedSets)
                {
                    Item setItem = new Item() { name = set.name, attributes = set.getBonusAttributes(set.countItemsOfSet(items)), displayColor = "green" };
                    if (setItem.attributes.Length > 0)
                        gearAttr.AddRange(getDataForItem(Resource.String.setBonuses, setItem, null));
                }

                heroGearListView.Adapter = new SectionedListAdapter(Activity, gearAttr.ToArray());
            }
        }
    }
}