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
using MagicParagons.Towers;

[assembly: MelonInfo(typeof(MagicParagons.Main), "Magic Paragons", "1.0.0", "DepletedNova")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace MagicParagons
{
    class Main : BloonsTD6Mod
    {
        static Dictionary<string, Type> Paragons = new Dictionary<string, Type>()
        {
            { "WizardMonkey", typeof(WizardParagon) }
        };

        //! Update
        [HarmonyPatch(typeof(InGame), nameof(InGame.Update))]
        class Update
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
                            foreach (KeyValuePair<string, Type> pair in Paragons)
                            {
                                if (pair.Key == towerModel.baseId)
                                {

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        //! Tower upgraded to Paragon
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

                        }
                        break;
                    }
                }
            }
        }

        //! DO NOT CHANGE
        static List<Tuple<TowerModel, UpgradeModel>> enabledParagons = new List<Tuple<TowerModel, UpgradeModel>>();

        static List<ModSettingBool> paragonSettings = new List<ModSettingBool>();

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            foreach (KeyValuePair<string, Type> pair in Paragons)
                paragonSettings.Add(new ModSettingBool(true) { displayName = $"Enable {pair.Key} Paragon? (Requires restart.)" });
            MelonLogger.Msg("Settings loaded.");
        }

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

        [HarmonyPatch(typeof(Btd6Player), nameof(Btd6Player.CheckShowParagonPip))]
        class Btd6Player_CheckShowParagonPip
        {
            [HarmonyPrefix]
            internal static bool Prefix(string towerId)
            {
                bool isCustomParagon = false;
                foreach (var paragonPair in enabledParagons)
                {
                    if (towerId == paragonPair.Item1.baseId)
                    {
                        isCustomParagon = true;
                    }
                }
                return !isCustomParagon;
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
