using Assets.Main.Scenes;
using Assets.Scripts.Models;

using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;

using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Unity.Player;
using Assets.Scripts.Unity.UI_New.InGame;

using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;

using HarmonyLib;
using UnhollowerBaseLib;
using MelonLoader;

using System;
using System.Collections.Generic;
using System.Linq;

using MagicParagons.Towers;
using Assets.Scripts.Simulation.Towers;

[assembly: MelonInfo(typeof(MagicParagons.Main), "Magic Paragons", "0.3.0", "DepletedNova")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace MagicParagons
{
    class Main : BloonsTD6Mod
    {
        static Dictionary<string, Type> Paragons = new Dictionary<string, Type>()
        {
            { "WizardMonkey", typeof(WizardParagon) },
            { "SuperMonkey", typeof(SuperParagon) },
            { "Druid", typeof(DruidParagon) },
            { "Alchemist", typeof(AlchemistParagon) }
        };

        // Individual mod settings
        static ModSettingBool wizardParagon = new ModSettingBool(true) { displayName = "WizardMonkey Paragon enabled? (Requires restart.)" };
        static ModSettingBool superParagon = new ModSettingBool(true) { displayName = "SuperMonkey Paragon enabled? (Requires restart.)" };
        static ModSettingBool druidParagon = new ModSettingBool(true) { displayName = "Druid Paragon enabled? (Requires restart.)" };
        static ModSettingBool alchemistParagon = new ModSettingBool(true) { displayName = "Alchemist Paragon enabled? (Requires restart.)" };

        // Mod setting pool
        static List<ModSettingBool> paragonSettings = new List<ModSettingBool>()
        {wizardParagon, superParagon, druidParagon, alchemistParagon};

        // On Round start
        [HarmonyPatch(typeof(InGame),nameof(InGame.RoundStart))]
        class RoundStart
        {
            [HarmonyPostfix]
            internal static void Postfix()
            {
                if (InGame.instance?.bridge != null)
                {
                    var towers = InGame.instance.bridge.GetAllTowers();
                    foreach (var simTower in towers)
                    {
                        var towerModel = simTower.tower.towerModel;
                        if (simTower.IsParagon)
                        {

                        }
                    }
                }
            }
        }

        // Degree updated
        [HarmonyPatch(typeof(ParagonTower),nameof(ParagonTower.UpdateDegree))]
        class UpdateDegreePatch
        {
            [HarmonyPostfix]
            internal static void Postfix(ParagonTower __instance)
            {
                var tower = __instance.tower;
                var towerModel = tower.towerModel;
                var degree = tower.GetTowerBehavior<ParagonTower>().GetCurrentDegree();
                if (towerModel.baseId == "Druid")
                {
                    if (degree > 70)
                    {
                        var filters = new Il2CppReferenceArray<FilterModel>(new FilterModel[]
                        {
                                        new FilterWithTagModel("MoabFilter","Moabs",false),
                                        new FilterInvisibleModel("InvisFilter",false,false)
                        });
                        towerModel.GetAttackModel().GetBehavior<AttackFilterModel>().filters = filters;
                        towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<ProjectileFilterModel>()
                            .filters = filters;
                    }
                    var spiritModel = towerModel.GetBehavior<SpiritOfTheForestModel>();
                    int mathDegree = (int)Math.Floor(new Decimal(degree / 5));
                    spiritModel.damageOverTimeZoneModelFar.behaviorModel.damage = 8 + mathDegree;
                    spiritModel.damageOverTimeZoneModelMiddle.behaviorModel.damage = 15 + mathDegree;
                    spiritModel.damageOverTimeZoneModelClose.behaviorModel.damage = 30 + mathDegree;
                }
            }
        }

        // Model loading
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            // Put models here
        }

        // Upgrade to Paragon
        [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.UpgradeTowerParagon))]
        class UpgradeTowerParagon
        {
            [HarmonyPostfix]
            internal static void Postfix(int id)
            {
                foreach (var simTower in InGame.instance.UnityToSimulation.GetAllTowers())
                {
                    if (simTower.tower.Id == id)
                    {
                        if (simTower.IsParagon)
                        {
                            // Pheonix fix
                            if (simTower.tower.towerModel.baseId == "WizardMonkey")
                            {
                                foreach (var simulatedTower in InGame.instance.UnityToSimulation.GetAllTowers())
                                {
                                    if (simulatedTower.tower.towerModel.baseId == "PermaPhoenix" ||
                                        simulatedTower.tower.towerModel.baseId == "WizardLordPhoenix")
                                    {
                                        simulatedTower.tower.Destroy();
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        //! Base function
        static List<Tuple<TowerModel, UpgradeModel>> enabledParagons = new List<Tuple<TowerModel, UpgradeModel>>();

        [HarmonyPatch(typeof(ProfileModel), nameof(ProfileModel.Validate))]
        class ProfileModel_Validate
        {
            [HarmonyPostfix]
            internal static void Postfix(ProfileModel __instance)
            {
                foreach (var paragonPair in enabledParagons)
                {
                    __instance.unlockedTowers.Add(paragonPair.Item1.name);

                    __instance.acquiredUpgrades.Add(paragonPair.Item2.name);
                }
            }
        }

        [HarmonyPatch(typeof(TitleScreen), "Start")]
        class TitleStart
        {
            [HarmonyPostfix]
            internal static void Postfix()
            {
                foreach (KeyValuePair<string, Type> pair in Paragons)
                {
                    foreach (var modSetting in paragonSettings)
                    {
                        if (modSetting && modSetting.displayName.Contains(pair.Key))
                        {
                            enabledParagons.Add(new Tuple<TowerModel, UpgradeModel>(
                                (TowerModel)pair.Value.GetField("Tower").GetValue(null),
                                (UpgradeModel)pair.Value.GetField("Upgrade").GetValue(null)
                            ));
                            Game.instance.model.AddUpgrade(enabledParagons.Last().Item2);
                            Game.instance.model.AddTowerToGame(enabledParagons.Last().Item1);
                            
                            MelonLogger.Msg(pair.Key + " Paragon loaded!");
                            break;
                        }
                    }
                    Game.instance.GetLocalizationManager().textTable[$"{pair.Key} Paragon"] =
                        (string)pair.Value.GetField("DisplayName").GetValue(null);
                    Game.instance.GetLocalizationManager().textTable[$"{pair.Key} Paragon Description"] =
                        (string)pair.Value.GetField("Description").GetValue(null);
                }
            }
        }

        [HarmonyPatch(typeof(GameModel), nameof(GameModel.CreateModded), new Type[] { typeof(GameModel), typeof(Il2CppSystem.Collections.Generic.List<ModModel>) })]
        class GameModel_CreateModded
        {
            [HarmonyPostfix]
            internal static void Postfix(GameModel result)
            {
                foreach (var paragonPair in enabledParagons)
                {
                    string baseTower = paragonPair.Item1.baseId;

                    for (int tier = 0; tier <= 2; tier++)
                    {
                        result.GetTower($"{baseTower}", 5, tier, 0).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                        result.GetTower($"{baseTower}", 5, 0, tier).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                        result.GetTower($"{baseTower}", tier, 5, 0).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                        result.GetTower($"{baseTower}", 0, 5, tier).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                        result.GetTower($"{baseTower}", tier, 0, 5).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                        result.GetTower($"{baseTower}", 0, tier, 5).paragonUpgrade = new UpgradePathModel(upgrade: $"{baseTower} Paragon", tower: $"{baseTower}-Paragon");
                    }
                }
            }
        }
    }
}
