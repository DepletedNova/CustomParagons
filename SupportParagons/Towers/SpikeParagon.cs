using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using UnhollowerBaseLib;

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

            AddCustomBehaviors();

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
            towerModel.isGlobalRange = true;
            towerModel.areaTypes = towers[0].areaTypes;

            towerModel.tier = 6;
            towerModel.tiers = Game.instance.model.GetTowerFromId("DartMonkey-Paragon").tiers;

            towerModel.icon = towers[1].icon;
            towerModel.portrait = towers[1].portrait;
            towerModel.instaIcon = towers[1].instaIcon;

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

        static void AddCustomBehaviors()
        {
            // Base Mines
            towerModel.AddBehavior(towers[1].GetAttackModel().Duplicate());
            var attackModel = towerModel.GetAttackModel();
            attackModel.weapons[0].rate = 1f;

            // Perma-mine behavior
            var ageModel = attackModel.weapons[0].projectile.GetBehavior<AgeModel>();
            ageModel.useRoundTime = true; ageModel.rounds = 15;
            attackModel.weapons[0].projectile.AddBehavior(towers[3].GetWeapon().projectile
                .GetBehavior<EndOfRoundClearBypassModel>().Duplicate());

            // Funny mines
            towerModel.AddBehavior(Game.instance.model.GetTower("SpikeFactory",2).GetAttackModel().Duplicate());
            var globalAttackModel = towerModel.GetAttackModels()[1];
            globalAttackModel.range = 9999; globalAttackModel.weapons[0].rate = 0.025f;
            globalAttackModel.weapons[0].projectile.GetDamageModel().damage = 4;
            globalAttackModel.weapons[0].animateOnMainAttack = false;
            var globalAgeModel = globalAttackModel.weapons[0].projectile.GetBehavior<AgeModel>();
            globalAgeModel.useRoundTime = false; globalAgeModel.lifespan = 10f;
            globalAttackModel.weapons[0].projectile.AddBehavior(towers[3].GetWeapon().projectile
                .GetBehavior<EndOfRoundClearBypassModel>().Duplicate());
            globalAttackModel.weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModiferForTag_SpikeParagon",
                "Ceramic", 2.5f, 0, true, true));
        }

        static void CustomizeTower()
        {
            var boomerangParagon = Game.instance.model.GetTowerFromId("BoomerangMonkey-Paragon").Duplicate();

            towerModel.display = towers[1].display;
            towerModel.GetBehavior<DisplayModel>().display = towers[1].display;

            towerModel.AddBehavior(boomerangParagon.GetBehavior<ParagonTowerModel>());
            towerModel.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = towers[1].display);
            towerModel.AddBehavior(boomerangParagon.GetBehavior<CreateSoundOnAttachedModel>());
        }
    }
}
