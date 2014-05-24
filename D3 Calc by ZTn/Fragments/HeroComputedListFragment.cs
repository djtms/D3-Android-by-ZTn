using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Android.OS;
using Android.Views;
using Android.Widget;
using ZTn.BNet.D3.Helpers;
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
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            base.OnCreate(savedInstanceState);

            RetainInstance = true;
        }

        /// <inheritdoc/>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            var view = inflater.Inflate(Resource.Layout.ViewHero, container, false);

            UpdateView(view);

            return view;
        }

        #endregion

        private static ZTn.BNet.D3.Calculator.D3Calculator GetCalculator()
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            var hero = D3Context.Instance.CurrentHero;
            var heroItems = D3Context.Instance.CurrentHeroItems;

            // Retrieve worn items
            var items = new List<Item>
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
                D3Context.Instance.ActivatedSetBonus
            };
            items = items.Where(i => i != null).Select(i => i.DeepClone()).ToList();

            if (heroItems.mainHand == null)
            {
                heroItems.mainHand = new Item(new ItemAttributes());
            }
            if (heroItems.offHand == null)
            {
                heroItems.offHand = new Item(new ItemAttributes());
            }

            var d3Calculator = new ZTn.BNet.D3.Calculator.D3Calculator(hero, ((Item)heroItems.mainHand).DeepClone(), ((Item)heroItems.offHand).DeepClone(), items.ToArray());

            return d3Calculator;
        }

        private void UpdateView(View view)
        {
            ZTnTrace.Trace(MethodBase.GetCurrentMethod());

            var hero = D3Context.Instance.CurrentHero;
            var heroItems = D3Context.Instance.CurrentHeroItems;

            if (hero == null || heroItems == null)
            {
                return;
            }

            var d3Calculator = GetCalculator();
            var dps = d3Calculator.GetHeroDps(new List<ZTn.BNet.D3.Calculator.Skills.ID3SkillModifier>(), new List<ZTn.BNet.D3.Calculator.Skills.ID3SkillModifier>()).Min;

            var attr = d3Calculator.HeroStatsItem.AttributesRaw;

            var heroStatsListView = view.FindViewById<ListView>(Resource.Id.heroStatsListView);
            var characteristicsAttr = new List<IListItem>
            {
                new SectionHeaderListItem(Resource.String.Progress),
                new AttributeListItem(Resource.String.heroClass, hero.heroClass),
                new AttributeListItem(Resource.String.level, hero.level),
                new AttributeListItem(Resource.String.paragon, hero.paragonLevel),
                new SectionHeaderListItem(Resource.String.attributes),
                new AttributeListItem(Resource.String.dexterity, d3Calculator.GetHeroDexterity()),
                new AttributeListItem(Resource.String.intelligence, d3Calculator.GetHeroIntelligence()),
                new AttributeListItem(Resource.String.strength, d3Calculator.GetHeroStrength()),
                new AttributeListItem(Resource.String.vitality, d3Calculator.GetHeroVitality()),
                new SectionHeaderListItem(Resource.String.damages),
                new AttributeListItem(Resource.String.damage, dps),
            };

            if (attr.critPercentBonusCapped != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.criticChance, attr.critPercentBonusCapped));
            }
            if (attr.critDamagePercent != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.criticDamage, attr.critDamagePercent + 1));
            }

            characteristicsAttr.Add(new AttributePercentListItem(Resource.String.attackSpeed, d3Calculator.GetActualAttackSpeed()));

            if (attr.damageDealtPercentBonusArcane != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Arcane, attr.damageDealtPercentBonusArcane));
            }
            if (attr.damageDealtPercentBonusCold != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Cold, attr.damageDealtPercentBonusCold));
            }
            if (attr.damageDealtPercentBonusFire != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Fire, attr.damageDealtPercentBonusFire));
            }
            if (attr.damageDealtPercentBonusHoly != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Holy, attr.damageDealtPercentBonusHoly));
            }
            if (attr.damageDealtPercentBonusLightning != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Lightning, attr.damageDealtPercentBonusLightning));
            }
            if (attr.damageDealtPercentBonusPhysical != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Physical, attr.damageDealtPercentBonusPhysical));
            }
            if (attr.damageDealtPercentBonusPoison != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.damageDealtPercent_Poison, attr.damageDealtPercentBonusPoison));
            }

            characteristicsAttr.AddRange(new List<IListItem>
            {
                new SectionHeaderListItem(Resource.String.life),
                new AttributeListItem(Resource.String.life, d3Calculator.GetHeroHitpoints())
            });

            if (attr.hitpointsOnHit != null)
            {
                characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeOnHit, attr.hitpointsOnHit));
            }
            if (attr.stealHealthPercent != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.lifeSteal, attr.stealHealthPercent));
            }
            if (attr.hitpointsOnKill != null)
            {
                characteristicsAttr.Add(new AttributeListItem(Resource.String.lifePerKill, attr.hitpointsOnKill));
            }
            if (attr.healthGlobeBonusHealth != null)
            {
                characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeBonusPerGlobe, attr.healthGlobeBonusHealth));
            }
            if (attr.hitpointsRegenPerSecond != null)
            {
                characteristicsAttr.Add(new AttributeListItem(Resource.String.lifeRegenPerSecond, attr.hitpointsRegenPerSecond));
            }

            characteristicsAttr.AddRange(new List<IListItem>
            {
                new AttributeListItem(Resource.String.effectiveHitpoints, Math.Round(d3Calculator.GetHeroEffectiveHitpoints(hero.level + 3))),
                new AttributeListItem(Resource.String.EHP_DPS, Math.Round((d3Calculator.GetHeroEffectiveHitpoints(hero.level + 3)*d3Calculator.GetHeroDps()).Min/1000000)),
                new SectionHeaderListItem(Resource.String.defense),
                new AttributeListItem(Resource.String.dodge, d3Calculator.GetHeroDodge()),
                new AttributeListItem(Resource.String.armor, Math.Round(d3Calculator.GetHeroArmor().Min)),
                new AttributeListItem(Resource.String.arcaneResist, d3Calculator.GetHeroResistance("Arcane")),
                new AttributeListItem(Resource.String.coldResist, d3Calculator.GetHeroResistance("Cold")),
                new AttributeListItem(Resource.String.fireResist, d3Calculator.GetHeroResistance("Fire")),
                new AttributeListItem(Resource.String.lightningResist, d3Calculator.GetHeroResistance("Lightning")),
                new AttributeListItem(Resource.String.physicalResist, d3Calculator.GetHeroResistance("Physical")),
                new AttributeListItem(Resource.String.poisonResist, d3Calculator.GetHeroResistance("Poison")),
                new SectionHeaderListItem(Resource.String.bonuses)
            });

            if (attr.goldFind != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.goldFind, attr.goldFind));
            }
            if (attr.magicFind != null)
            {
                characteristicsAttr.Add(new AttributePercentListItem(Resource.String.magicFind, attr.magicFind));
            }

            heroStatsListView.Adapter = new ListAdapter(Activity, characteristicsAttr.ToArray());
        }
    }
}