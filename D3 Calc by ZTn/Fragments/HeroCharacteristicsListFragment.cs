using System;
using System.Collections.Generic;
using System.Reflection;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using ZTn.BNet.D3.Heroes;
using ZTnDroid.D3Calculator.Adapters;
using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;

using Fragment = Android.Support.V4.App.Fragment;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class HeroCharacteristicsListFragment : ZTnFragment
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

            View view = inflater.Inflate(Resource.Layout.ViewHero, container, false);

            updateView(view);

            return view;
        }

        private void updateView(View view)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Hero hero = D3Context.instance.hero;
            if (hero != null)
            {
                ListView heroStatsListView = view.FindViewById<ListView>(Resource.Id.heroStatsListView);
                List<IListItem> characteristicsAttr = new List<IListItem>()
                {
                    new SectionHeaderListItem(Resource.String.Progress),
                    new AttributeListItem(Resource.String.lastUpdated, hero.lastUpdated),
                    new AttributeListItem(Resource.String.heroClass, hero.heroClass),
                    new AttributeListItem(Resource.String.level, hero.level),
                    new AttributeListItem(Resource.String.paragon, hero.paragonLevel),

                    new SectionHeaderListItem(Resource.String.KillsLifetime),
                    new AttributeListItem(Resource.String.elites, hero.kills.elites),

                    new SectionHeaderListItem(Resource.String.attributes),
                    new AttributeListItem(Resource.String.dexterity, hero.stats.dexterity),
                    new AttributeListItem(Resource.String.intelligence, hero.stats.intelligence),
                    new AttributeListItem(Resource.String.strength, hero.stats.strength),
                    new AttributeListItem(Resource.String.vitality, hero.stats.vitality),

                    new SectionHeaderListItem(Resource.String.damages),
                    new AttributeListItem(Resource.String.damage, hero.stats.damage),
                    new AttributePercentListItem(Resource.String.criticChance, hero.stats.critChance),
                    new AttributePercentListItem(Resource.String.criticDamage, hero.stats.critDamage),
                    new AttributePercentListItem(Resource.String.attackSpeed , hero.stats.attackSpeed),

                    new SectionHeaderListItem(Resource.String.life),
                    new AttributeListItem(Resource.String.life, hero.stats.life),
                    new AttributeListItem(Resource.String.lifeOnHit, hero.stats.lifeOnHit),
                    new AttributePercentListItem(Resource.String.lifeSteal, hero.stats.lifeSteal),
                    new AttributeListItem(Resource.String.lifePerKill, hero.stats.lifePerKill),

                    new SectionHeaderListItem(Resource.String.defense),
                    new AttributeListItem(Resource.String.armor, hero.stats.armor),
                    new AttributeListItem(Resource.String.arcaneResist, hero.stats.arcaneResist),
                    new AttributeListItem(Resource.String.coldResist, hero.stats.coldResist),
                    new AttributeListItem(Resource.String.fireResist, hero.stats.fireResist),
                    new AttributeListItem(Resource.String.lightningResist, hero.stats.lightningResist),
                    new AttributeListItem(Resource.String.physicalResist, hero.stats.physicalResist),
                    new AttributeListItem(Resource.String.poisonResist, hero.stats.poisonResist),

                    new SectionHeaderListItem(Resource.String.resources),
                    new AttributeListItem(Resource.String.primaryResource, hero.stats.primaryResource),
                    new AttributeListItem(Resource.String.secondaryResource,hero.stats.secondaryResource),

                    new SectionHeaderListItem(Resource.String.bonuses),
                    new AttributePercentListItem(Resource.String.goldFind, hero.stats.goldFind),
                    new AttributePercentListItem(Resource.String.magicFind, hero.stats.magicFind)
                };
                heroStatsListView.Adapter = new SectionedListAdapter(Activity, characteristicsAttr.ToArray());
            }
        }
    }
}