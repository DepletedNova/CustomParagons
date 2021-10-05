using Assets.Main.Scenes;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Player;

using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;

using HarmonyLib;
using MelonLoader;

using System;
using System.Collections.Generic;
using System.Linq;

using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Utils;
using UnhollowerBaseLib;
using Assets.Scripts.Models.GenericBehaviors;

namespace MagicParagons
{
    abstract class ModdedParagon
    {
        protected static void setupTower(ref UpgradeModel upgradeModel, ref TowerModel towerModel, string TowerClass, string BaseTower,
            float Price, string Icon, string Display)
        {
            upgradeModel = new UpgradeModel(
                name: $"{BaseTower} Paragon",
                cost: (int)Price,
                xpCost: 0,
                icon: new SpriteReference(guid: Icon),
                path: -1,
                tier: 5,
                locked: 0,
                confirmation: "Paragon",
                localizedNameOverride: ""
            );

            var baseTower = Game.instance.model.GetTower(BaseTower,5).Duplicate();

            towerModel = new TowerModel();

            towerModel.name = $"{BaseTower}-Paragon";
            towerModel.display = Display;
            towerModel.baseId = BaseTower;

            towerModel.cost = Price;
            towerModel.towerSet = TowerClass;
            towerModel.radius = 6f;
            towerModel.radiusSquared = 36f;
            towerModel.range = 50f;

            towerModel.ignoreBlockers = true;
            towerModel.isGlobalRange = false;
            towerModel.areaTypes = baseTower.areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = baseTower.icon;
            towerModel.portrait = baseTower.portrait;
            towerModel.instaIcon = baseTower.instaIcon;

            towerModel.ignoreTowerForSelection = false;
            towerModel.footprint = baseTower.footprint.Duplicate();
            towerModel.dontDisplayUpgrades = false;
            towerModel.emoteSpriteSmall = null;
            towerModel.emoteSpriteLarge = null;
            towerModel.doesntRotate = false;

            towerModel.upgrades = new Il2CppReferenceArray<UpgradePathModel>(0);
            var appliedUpgrades = new Il2CppStringArray(6);
            for (int upgrade = 0; upgrade < 5; upgrade++)
            {
                appliedUpgrades[upgrade] = baseTower.appliedUpgrades[upgrade];
            }
            appliedUpgrades[5] = $"{BaseTower} Paragon";
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

            towerModel.mods = new Il2CppReferenceArray<ApplyModModel>(0);
            towerModel.mods = towerModel.mods.AddTo(baseTower.mods[0]); towerModel.mods = towerModel.mods.AddTo(baseTower.mods[1]);

            towerModel.AddBehavior(baseTower.GetBehavior<CreateEffectOnPlaceModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<CreateSoundOnTowerPlaceModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<CreateSoundOnUpgradeModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<CreateSoundOnSellModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<CreateEffectOnSellModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<CreateEffectOnUpgradeModel>());
            towerModel.AddBehavior(baseTower.GetBehavior<DisplayModel>());

            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = Display;
            towerModel.GetBehavior<DisplayModel>().display = Display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = Display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }

    abstract class ModParagonInfo
    {
        protected const string PRIMARY = "Primary";
        protected const string MILITARY = "Military";
        protected const string MAGIC = "Magic";
        protected const string SUPPORT = "Support";

        public abstract float Price { get; }
        public abstract string BaseTower { get; }
        public abstract string TowerClass { get; }

        public virtual string Icon => Game.instance.model.GetTower("DartMonkey").icon.GUID;
        public virtual string Display => Game.instance.model.GetTower("DartMonkey").display;

        protected const string DART = "DartMonkey";
        protected const string BOOMERANG = "BoomerangMonkey";
        protected const string BOMB = "BombShooter";
        protected const string TACK = "TackShooter";
        protected const string ICE = "IceMonkey";
        protected const string GLUE = "GlueGunner";
        protected const string SNIPER = "SniperMonkey";
        protected const string SUB = "MonkeySub";
        protected const string BOAT = "MonkeyBuccaneer";
        protected const string ACE = "MonkeyAce";
        protected const string HELI = "HeliPilot";
        protected const string MORTAR = "MortarMonkey";
        protected const string DARTLING = "DartlingGunner";
        protected const string WIZARD = "WizardMonkey";
        protected const string SUPER = "SuperMonkey";
        protected const string NINJA = "NinjaMonkey";
        protected const string ALCH = "Alchemist";
        protected const string DRUID = "Druid";
        protected const string FARM = "BananaFarm";
        protected const string SPIKE = "SpikeFactory";
        protected const string VILLAGE = "MonkeyVillage";
        protected const string ENGINEER = "EngineerMonkey";
    }

    class ParagonHelper
    {
        
    }
}
