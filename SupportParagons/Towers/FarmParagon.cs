using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.TowerFilters;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Unity;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using SupportParagons.Towers.Sub;
using UnhollowerBaseLib;

namespace SupportParagons.Towers
{
    class FarmParagon
    {
        static float price = 300000f;

        public static TowerModel towerModel;
        public static UpgradeModel upgradeModel;

        public static string baseTower = "BananaFarm";

        static FarmParagon()
        {
            CreateUpgrade();

            InitializeTower();

            AddGenericBehaviors();

            AddCommonBehaviors();

            CustomizeTower();
        }

        static TowerModel[] farms =
        {
            Game.instance.model.GetTower("BananaFarm").Duplicate(),
            Game.instance.model.GetTower("BananaFarm",5).Duplicate(),
            Game.instance.model.GetTower("BananaFarm",0,5).Duplicate(),
            Game.instance.model.GetTower("BananaFarm",0,0,5).Duplicate(),
        };

        static void CreateUpgrade()
        {
            upgradeModel = new UpgradeModel(
                name: "BananaFarm Paragon",
                cost: (int)price,
                xpCost: 0,
                icon: farms[0].icon,
                path: -1,
                tier: 5,
                locked: 0,
                confirmation: "Paragon",
                localizedNameOverride: ""
            );;
        }

        static void InitializeTower()
        {
            towerModel = new TowerModel();

            towerModel.name = "BananaFarm-Paragon";
            towerModel.display = farms[0].display;
            towerModel.baseId = "BananaFarm";

            towerModel.cost = price;
            towerModel.towerSet = "Support";
            towerModel.radius = 6f;
            towerModel.radiusSquared = 36f;
            towerModel.range = 100f;

            towerModel.ignoreBlockers = true;
            towerModel.isGlobalRange = false;
            towerModel.areaTypes = farms[0].areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = farms[0].icon;
            towerModel.portrait = farms[2].portrait;
            towerModel.instaIcon = farms[2].instaIcon;

            towerModel.ignoreTowerForSelection = false;
            towerModel.footprint = farms[0].footprint.Duplicate();
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
            appliedUpgrades[5] = "BananaFarm Paragon";
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
            towerModel.mods = towerModel.mods.AddTo(farms[1].mods[0]); towerModel.mods = towerModel.mods.AddTo(farms[1].mods[2]);
            towerModel.mods = towerModel.mods.AddTo(farms[1].mods[3]); towerModel.mods = towerModel.mods.AddTo(farms[1].mods[4]);

            towerModel.AddBehavior(farms[1].GetBehavior<CreateEffectOnPlaceModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<CreateSoundOnTowerPlaceModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<CreateSoundOnUpgradeModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<CreateSoundOnSellModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<CreateEffectOnSellModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<CreateEffectOnUpgradeModel>());
            towerModel.AddBehavior(farms[1].GetBehavior<DisplayModel>());
        }

        static void AddCommonBehaviors()
        {
            var f005 = Game.instance.model.GetTower(TowerType.BananaFarm, 0, 0, 5);

            towerModel.AddBehavior(f005.GetAttackModel().Duplicate());
            var cashModel = towerModel.GetWeapon().projectile.GetBehavior<CashModel>();
            cashModel.minimum = 500f; cashModel.maximum = 500f;

            var e100 = Game.instance.model.GetTower("EngineerMonkey", 1);
            towerModel.AddBehavior(e100.GetAttackModel().Duplicate());
            var attackModel = towerModel.GetAttackModels()[1];

            attackModel.weapons[0].rate = 2.5f;
            attackModel.RemoveBehavior<RotateToTargetModel>(); attackModel.RemoveBehavior<CreateEffectWhileAttackingModel>();

            var tower = ModContent.GetTowerModel<MarketStand>();
            attackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = tower.Duplicate();
            attackModel.weapons[0].projectile.AddBehavior(new InstantModel("FarmParagon_Instant", false));
        }

        static void CustomizeTower()
        {
            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = farms[2].display;
            towerModel.GetBehavior<DisplayModel>().display = farms[2].display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = farms[2].display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }
}
