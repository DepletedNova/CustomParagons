using Assets.Scripts.Models;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api;
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
    class EngineerParagon
    {
        static float price = 350000f;

        public static TowerModel towerModel;
        public static UpgradeModel upgradeModel;

        public static string baseTower = "EngineerMonkey";

        static EngineerParagon()
        {
            CreateUpgrade();

            InitializeTower();

            AddGenericBehaviors();

            AddCommonBehaviors();

            CustomizeTower();
        }

        static TowerModel[] monkeys =
        {
            Game.instance.model.GetTower("EngineerMonkey").Duplicate(),
            Game.instance.model.GetTower("EngineerMonkey",5).Duplicate(),
            Game.instance.model.GetTower("EngineerMonkey",0,5).Duplicate(),
            Game.instance.model.GetTower("EngineerMonkey",0,0,5).Duplicate(),
        };

        static void CreateUpgrade()
        {
            var bomb = Game.instance.model.GetTower("EngineerMonkey", 0,5,0).Duplicate();
            upgradeModel = new UpgradeModel(
                name: "EngineerMonkey Paragon",
                cost: (int)price,
                xpCost: 0,
                icon: new SpriteReference(guid: bomb.icon.GUID),
                path: -1,
                tier: 5,
                locked: 0,
                confirmation: "Paragon",
                localizedNameOverride: ""
            );
        }

        static void InitializeTower()
        {
            towerModel = new TowerModel();

            towerModel.name = "EngineerMonkey-Paragon";
            towerModel.display = monkeys[1].display;
            towerModel.baseId = "EngineerMonkey";

            towerModel.cost = price;
            towerModel.towerSet = "Support";
            towerModel.radius = 6f;
            towerModel.radiusSquared = 36f;
            towerModel.range = 70f;

            towerModel.ignoreBlockers = false;
            towerModel.isGlobalRange = false;
            towerModel.areaTypes = monkeys[1].areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = monkeys[1].icon;
            towerModel.portrait = monkeys[1].portrait;
            towerModel.instaIcon = monkeys[1].instaIcon;

            towerModel.ignoreTowerForSelection = false;
            towerModel.footprint = monkeys[1].footprint.Duplicate();
            towerModel.dontDisplayUpgrades = false;
            towerModel.emoteSpriteSmall = null;
            towerModel.emoteSpriteLarge = null;
            towerModel.doesntRotate = true;

            towerModel.upgrades = new Il2CppReferenceArray<UpgradePathModel>(0);
            var appliedUpgrades = new Il2CppStringArray(6);
            for (int upgrade = 0; upgrade < 5; upgrade++)
            {
                appliedUpgrades[upgrade] = Game.instance.model.GetTower(TowerType.BananaFarm,5,0,0).appliedUpgrades[upgrade];
            }
            appliedUpgrades[5] = "EngineerMonkey Paragon";
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
            towerModel.mods = towerModel.mods.AddTo(monkeys[1].mods[0]); towerModel.mods = towerModel.mods.AddTo(monkeys[1].mods[1]);
            towerModel.mods = towerModel.mods.AddTo(monkeys[1].mods[3]);

            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateEffectOnPlaceModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateSoundOnTowerPlaceModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateSoundOnUpgradeModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateSoundOnSellModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateEffectOnSellModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<CreateEffectOnUpgradeModel>());
            towerModel.AddBehavior(monkeys[1].GetBehavior<DisplayModel>());
        }

        static void AddCommonBehaviors()
        {
            foreach (var model in monkeys[1].GetAttackModels()) towerModel.AddBehavior(model.Duplicate());
            var sentryAttackModel = towerModel.GetAttackModels()[0]; var attackModel = towerModel.GetAttackModels()[1];

            sentryAttackModel.weapons[0].rate = 2f; attackModel.range = 80; towerModel.range = 80;
            sentryAttackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = ModContent.GetTowerModel<OverclockedSentry>();
            attackModel.weapons[0].projectile.display = Game.instance.model.GetTower(TowerType.SentryParagon).GetWeapon().projectile.display;
        }

        static void CustomizeTower()
        {
            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = monkeys[1].display;
            towerModel.GetBehavior<DisplayModel>().display = monkeys[1].display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = monkeys[1].display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }
}
