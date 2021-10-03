using Assets.Scripts.Models;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.TowerFilters;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Assets.Scripts.Simulation.Towers.Weapons.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using Assets.Scripts.Unity.Towers.Weapons.Behaviors;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using MelonLoader;
using SupportParagons.Towers.Sub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;

namespace SupportParagons.Towers
{
    class VillageParagon
    {
        static float price = 800000f;

        public static TowerModel towerModel;
        public static UpgradeModel upgradeModel;

        public static string baseTower = "MonkeyVillage";

        static VillageParagon()
        {
            CreateUpgrade();

            InitializeTower();

            AddGenericBehaviors();

            AddCustomBehaviors();

            CustomizeTower();
        }

        static TowerModel[] towers =
        {
            Game.instance.model.GetTower("MonkeyVillage").Duplicate(),
            Game.instance.model.GetTower("MonkeyVillage",5).Duplicate(),
            Game.instance.model.GetTower("MonkeyVillage",0,5).Duplicate(),
            Game.instance.model.GetTower("MonkeyVillage",0,0,5).Duplicate(),
        };

        static void CreateUpgrade()
        {
            upgradeModel = new UpgradeModel(
                name: "MonkeyVillage Paragon",
                cost: (int)price,
                xpCost: 0,
                icon: towers[2].icon,
                path: -1,
                tier: 5,
                locked: 0,
                confirmation: "Paragon",
                localizedNameOverride: ""
            ); ;
        }

        static void InitializeTower()
        {
            towerModel = new TowerModel();

            towerModel.name = "MonkeyVillage-Paragon";
            towerModel.display = towers[2].display;
            towerModel.baseId = "MonkeyVillage";

            towerModel.cost = price;
            towerModel.towerSet = "Support";
            towerModel.radius = 6f;
            towerModel.radiusSquared = 36f;
            towerModel.range = 80f;

            towerModel.ignoreBlockers = true;
            towerModel.isGlobalRange = true;
            towerModel.areaTypes = towers[0].areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = towers[2].icon;
            towerModel.portrait = towers[2].portrait;
            towerModel.instaIcon = towers[2].instaIcon;

            towerModel.ignoreTowerForSelection = false;
            towerModel.footprint = towers[0].footprint.Duplicate();
            towerModel.dontDisplayUpgrades = false;
            towerModel.emoteSpriteSmall = null;
            towerModel.emoteSpriteLarge = null;
            towerModel.doesntRotate = true;

            towerModel.upgrades = new Il2CppReferenceArray<UpgradePathModel>(0);
            var appliedUpgrades = new Il2CppStringArray(6);
            for (int upgrade = 0; upgrade < 5; upgrade++)
            {
                appliedUpgrades[upgrade] = towers[1].appliedUpgrades[upgrade];
            }
            appliedUpgrades[5] = "MonkeyVillage Paragon";
            towerModel.appliedUpgrades = appliedUpgrades;

            towerModel.paragonUpgrade = null;
            towerModel.isSubTower = false;
            towerModel.isBakable = true;
            towerModel.powerName = null;
            towerModel.showPowerTowerBuffs = false;
            towerModel.animationSpeed = 1f;
            towerModel.towerSelectionMenuThemeId = "Default";
            towerModel.ignoreCoopAreas = false;
            towerModel.canAlwaysBeSold = false;
            towerModel.isParagon = true;
        }

        static void AddGenericBehaviors()
        {
            towerModel.mods = new Il2CppReferenceArray<ApplyModModel>(0);
            towerModel.mods = towerModel.mods.AddTo(towers[1].mods[0]); towerModel.mods = towerModel.mods.AddTo(towers[1].mods[2]);
            towerModel.mods = towerModel.mods.AddTo(towers[1].mods[3]); towerModel.mods = towerModel.mods.AddTo(towers[1].mods[4]);

            towerModel.AddBehavior(towers[1].GetBehavior<CreateEffectOnPlaceModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<CreateSoundOnTowerPlaceModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<CreateSoundOnUpgradeModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<CreateSoundOnSellModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<CreateEffectOnSellModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<CreateEffectOnUpgradeModel>());
            towerModel.AddBehavior(towers[1].GetBehavior<DisplayModel>());
        }

        static void AddCustomBehaviors()
        {
            // Discounts
            var discountVillage = Game.instance.model.GetTower("MonkeyVillage", 0, 0, 2).GetBehavior<DiscountZoneModel>();
            towerModel.AddBehavior(new DiscountZoneModel("DiscountZone_VillageParagon", 0.4F, 1,
                "VILPARA_DISCOUNT", "VILLAGEPARA", false, 5, discountVillage.buffLocsName, discountVillage.buffIconName));
            towerModel.AddBehavior(new FreeUpgradeSupportModel("FreeUpgradeSupport_VillageParagon",
                2, "VillageParagon:Discount", new Il2CppReferenceArray<TowerFilterModel>(0)));
            // Buffs
            var rateSupportModel = towers[1].GetBehavior<RateSupportModel>();
            towerModel.AddBehavior(new RateSupportModel("RateSupport_VillageParagon", 1.1f, true,
                "VillageParagon:Buff", true, 1, new Il2CppReferenceArray<TowerFilterModel>(0),
                rateSupportModel.buffLocsName, rateSupportModel.buffIconName));
            towerModel.AddBehavior(new RangeSupportModel("RangeSupport_VillageParagon", true, 1f, 10,
                "VillageParagon:Buff", new Il2CppReferenceArray<TowerFilterModel>(0), true,
                rateSupportModel.buffLocsName, rateSupportModel.buffIconName));
            towerModel.AddBehavior(new PierceSupportModel("PierceSupport_VillageParagon", true, 25,
                "VillageParagon:Buff", new Il2CppReferenceArray<TowerFilterModel>(0), true,
                rateSupportModel.buffLocsName, rateSupportModel.buffIconName));
            var visibilitySupport = towers[2].GetBehavior<VisibilitySupportModel>().Duplicate();
            var damageTypeSupport = towers[2].GetBehavior<DamageTypeSupportModel>().Duplicate();
            visibilitySupport.buffLocsName = rateSupportModel.buffLocsName; visibilitySupport.buffIconName = rateSupportModel.buffIconName;
            damageTypeSupport.buffLocsName = rateSupportModel.buffLocsName; damageTypeSupport.buffIconName = rateSupportModel.buffIconName;
            towerModel.AddBehavior(visibilitySupport); towerModel.AddBehavior(damageTypeSupport);
            // Money gain
            towerModel.AddBehavior(Game.instance.model.GetTower("BananaFarm", 0, 0, 3).GetAttackModel());
            var cashModel = towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CashModel>();
            cashModel.minimum = 300; cashModel.maximum = 300;
            towerModel.GetAttackModel().weapons[0].GetBehavior<EmissionsPerRoundFilterModel>().count = 15;
        }

        static void CustomizeTower()
        {
            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = towers[2].display;
            towerModel.GetBehavior<DisplayModel>().display = towers[2].display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = towers[2].display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }
}
