using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Android.OS;
using Android.Views;
using Android.Widget;

using ZTn.BNet.D3.Heroes;
using ZTn.BNet.D3.Items;

using ZTnDroid.D3Calculator.Adapters;
using ZTnDroid.D3Calculator.Adapters.Delegated;
using ZTnDroid.D3Calculator.Helpers;
using ZTnDroid.D3Calculator.Storage;

namespace ZTnDroid.D3Calculator.Fragments
{
    public class HeroComputedListFragment : UpdatableFragment
    {
        #region >> ZTnFragment

        /// <inheritdoc/>
        public override void OnCreate(Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            base.OnCreate(savedInstanceState);

            RetainInstance = true;
        }

        /// <inheritdoc/>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            View view = inflater.Inflate(Resource.Layout.ViewHero, container, false);

            updateView(view);

            return view;
        }

        #endregion

        private ZTn.BNet.D3.Calculator.D3Calculator getCalculator()
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Hero hero = D3Context.instance.hero;
            HeroItems heroItems = D3Context.instance.heroItems;

            // Retrieve worn items
            List<Item> items = new List<Item>()
            {
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
                D3Context.instance.activatedSetBonus
            };
            items = items.Where(i => i != null).ToList();

            if (heroItems.mainHand == null)
                heroItems.mainHand = new Item(new ItemAttributes());
            if (heroItems.offHand == null)
                heroItems.offHand = new Item(new ItemAttributes());

            ZTn.BNet.D3.Calculator.D3Calculator d3Calculator = new ZTn.BNet.D3.Calculator.D3Calculator(hero, (Item)heroItems.mainHand, (Item)heroItems.offHand, items.ToArray());

            return d3Calculator;
        }

        private void updateView(View view)
        {
            ZTnTrace.trace(MethodInfo.GetCurrentMethod());

            Hero hero = D3Context.instance.hero;
            HeroItems heroItems = D3Context.instance.heroItems;
            if (hero != null && heroItems != null)
            {
                ZTn.BNet.D3.Calculator.D3Calculator d3Calculator = getCalculator();
                double dps = d3Calculator.getHeroDPS(new List<ZTn.BNet.D3.Calculator.Skills.D3SkillModifier>(), new List<ZTn.BNet.D3.Calculator.Skills.D3SkillModifier>()).min;

                ItemAttributes attr = d3Calculator.heroStatsItem.attributesRaw;

                ListView heroStatsListView = view.FindViewById<ListView>(Resource.Id.heroStatsListView);
                List<IListItem> characteristicsAttr = new List<IListItem>()
                {
                    new SectionHeaderListItem(Resource.String.Progress),
                    new AttributeListItem(Resource.String.heroClass, hero.heroClass),
                    new AttributeListItem(Resource.String.level, hero.level),
                    new AttributeListItem(Resource.String.paragon, hero.paragonLevel),

                    new SectionHeaderListItem(Resource.String.attributes),
                    new AttributeListItem(Resource.String.dexterity, d3Calculator.getHeroDexterity()),
                    new AttributeListItem(Resource.String.intelligence, d3Calculator.getHeroIntelligence()),
                    new AttributeListItem(Resource.String.strength, d3Calculator.getHeroStrength()),
                    new AttributeListItem(Resource.String.vitality, d3Calculator.getHeroVitality()),

                    new SectionHeaderListItem(Resource.String.damages),
                    new AttributeListItem(Resource.String.damage, dps),
                };

                if (attr.critPercentBonusCapped != null)
                    characteristicsAttr.Add(new AttributePercentListItem(Resource.String.criticChance, attr.critPercentBonusCapped));
                if (attr.critDamagePercent != null)
                    characteristicsAttr.Add(new AttributePercentListItem(Resource.String.criticDamage, attr.critDamagePercent + 1));

                characteristicsAttr.AddRange(new List<IListItem>()
                {
                    new AttributePercentListItem(Resource.String.attackSpeed, d3Calculator.getActualAttackSpeed()),

                    new SectionHeaderListItem(Resource.String.life),
                    new AttributeListItem(Resource.String.life, d3Calculator.getHeroHitpoints())
                });

                if (attr.hitpointsOnHit != null)
                    characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeOnHit, attr.hitpointsOnHit));
                if (attr.stealHealthPercent != null)
                    characteristicsAttr.Add(new AttributePercentListItem(Resource.String.lifeSteal, attr.stealHealthPercent));
                if (attr.hitpointsOnKill != null)
                    characteristicsAttr.Add(new AttributeListItem(Resource.String.lifePerKill, attr.hitpointsOnKill));
                if (attr.healthGlobeBonusHealth != null)
                    characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeBonusPerGlobe, attr.healthGlobeBonusHealth));
                if (attr.hitpointsRegenPerSecond != null)
                    characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeRegenPerSecond, attr.hitpointsRegenPerSecond));

                characteristicsAttr.AddRange(new List<IListItem>()
                {
                    new AttributeListItem(Resource.String.effectiveHitpoints, Math.Round(d3Calculator.getHeroEffectiveHitpoints(hero.level+3))),
                    new AttributeListItem(Resource.String.EHP_DPS, Math.Round((d3Calculator.getHeroEffectiveHitpoints(hero.level+3) * d3Calculator.getHeroDPS()).min / 1000000)),
                    
                    new SectionHeaderListItem(Resource.String.defense),
                    new AttributeListItem(Resource.String.dodge, d3Calculator.getHeroDodge()),
                    new AttributeListItem(Resource.String.armor, Math.Round(d3Calculator.getHeroArmor().min)),
                    new AttributeListItem(Resource.String.arcaneResist, d3Calculator.getHeroResistance("Arcane")),
                    new AttributeListItem(Resource.String.coldResist, d3Calculator.getHeroResistance("Cold")),
                    new AttributeListItem(Resource.String.fireResist, d3Calculator.getHeroResistance("Fire")),
                    new AttributeListItem(Resource.String.lightningResist, d3Calculator.getHeroResistance("Lightning")),
                    new AttributeListItem(Resource.String.physicalResist, d3Calculator.getHeroResistance("Physical")),
                    new AttributeListItem(Resource.String.poisonResist, d3Calculator.getHeroResistance("Poison")),

                    new SectionHeaderListItem(Resource.String.bonuses)
                });

                if (attr.goldFind != null)
                    characteristicsAttr.Add(new AttributePercentListItem(Resource.String.goldFind, attr.goldFind));
                if (attr.magicFind != null)
                    characteristicsAttr.Add(new AttributePercentListItem(Resource.String.magicFind, attr.magicFind));

                heroStatsListView.Adapter = new ListAdapter(Activity, characteristicsAttr.ToArray());
            }
        }
    }
}