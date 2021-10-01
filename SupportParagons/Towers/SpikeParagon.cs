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
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Simulation.Towers.Projectiles.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
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
using CreateEffectOnExpireModel = Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel;

namespace SupportParagons.Towers
{
    class SpikeParagon
    {
        static float price = 450000f;

        public static TowerModel towerModel;
        public static UpgradeModel upgradeModel;

        public static string baseTower = "SpikeFactory";

        static SpikeParagon()
        {
            CreateUpgrade();

            InitializeTower();

            AddGenericBehaviors();

            AddCommonBehaviors();

            CustomizeTower();
        }

        static TowerModel[] towers =
        {
            Game.instance.model.GetTower("SpikeFactory").Duplicate(),
            Game.instance.model.GetTower("SpikeFactory",5).Duplicate(),
            Game.instance.model.GetTower("SpikeFactory",0,5).Duplicate(),
            Game.instance.model.GetTower("SpikeFactory",0,0,5).Duplicate(),
        };

        static void CreateUpgrade()
        {
            upgradeModel = new UpgradeModel(
                name: "SpikeFactory Paragon",
                cost: (int)price,
                xpCost: 0,
                icon: towers[0].icon,
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

            towerModel.name = "SpikeFactory-Paragon";
            towerModel.display = towers[0].display;
            towerModel.baseId = "SpikeFactory";

            towerModel.cost = price;
            towerModel.towerSet = "Support";
            towerModel.radius = 6f;
            towerModel.radiusSquared = 36f;
            towerModel.range = 70f;

            towerModel.ignoreBlockers = true;
            towerModel.isGlobalRange = false;
            towerModel.areaTypes = towers[0].areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = towers[0].icon;
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
                appliedUpgrades[upgrade] = Game.instance.model.GetTower(TowerType.SpikeFactory, 5, 0, 0).appliedUpgrades[upgrade];
            }
            appliedUpgrades[5] = "SpikeFactory Paragon";
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

        static void AddCommonBehaviors()
        {
            // Plasma base attack
            towerModel.AddBehavior(towers[3].GetAttackModel().Duplicate());
            var mainAttackModel = towerModel.GetAttackModels()[0];
            var sentryParagon = Game.instance.model.GetTower("SentryParagon");
            var projectile = mainAttackModel.weapons[0].projectile = towers[0].GetWeapon().projectile.Duplicate();
            projectile.RemoveBehavior<SetSpriteFromPierceModel>();
            projectile.display = sentryParagon.GetWeapon().projectile.display;

            mainAttackModel.range = 60f; towerModel.range = 60f; mainAttackModel.weapons[0].rate = 0.35f;
            projectile.GetDamageModel().damage = 5f; projectile.pierce = 150f;
            projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            projectile.SetHitCamo(true);

            // Plasma burst
            towerModel.AddBehavior(Game.instance.model.GetTower("TackShooter").GetAttackModel().Duplicate());
            var burstAttackModel = towerModel.GetAttackModels()[1];
            burstAttackModel.weapons[0].projectile =
                sentryParagon.GetBehavior<CreateProjectileOnTowerDestroyModel>().projectileModel
                .GetBehavior<CreateProjectileOnExpireModel>().projectile.Duplicate();
            burstAttackModel.weapons[0].projectile.AddBehavior(sentryParagon.GetBehavior<CreateProjectileOnTowerDestroyModel>()
                .projectileModel.GetBehavior<CreateEffectOnExpireModel>().Duplicate());
            burstAttackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            burstAttackModel.fireWithoutTarget = true; burstAttackModel.weapons[0].rate = 1.5f;
            burstAttackModel.weapons[0].startInCooldown = true; burstAttackModel.weapons[0].projectile.SetHitCamo(true);
            burstAttackModel.weapons[0].projectile.pierce = 15f; burstAttackModel.weapons[0].projectile.GetDamageModel().damage = 1000;
            burstAttackModel.weapons[0].projectile.GetDamageModel().distributeToChildren = true;
        }

        static void CustomizeTower()
        {
            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = towers[3].display;
            towerModel.GetBehavior<DisplayModel>().display = towers[3].display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = towers[1].display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }
}
