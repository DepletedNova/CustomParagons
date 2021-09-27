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

using SupportParagons.Towers;
using Assets.Scripts.Unity.UI_New.InGame;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;

[assembly: MelonInfo(typeof(SupportParagons.Main), "Support Paragons", "1.0.0", "DepletedNova")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace SupportParagons
{
    public class Main : BloonsTD6Mod
    {
        //! Paragons
        static Dictionary<string, Type> paragonKeys = new Dictionary<string, Type>()
        {
            { "BananaFarm", typeof(FarmParagon) },
            { "EngineerMonkey", typeof(EngineerParagon) }
        };

        static List<Tuple<TowerModel, UpgradeModel>> paragons = new List<Tuple<TowerModel, UpgradeModel>>();

        static ModSettingBool paragonsEnabled = new ModSettingBool(true) {displayName = "Support Paragons enabled? (Requires restart)"};

        //! Application open
        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
        }

        //! App open
        [HarmonyPatch(typeof(TitleScreen), "Start")]
        class TitleScreen_Start
        {
            [HarmonyPostfix]
            internal static void Postfix()
            {
                // Adding paragons to game
                foreach (KeyValuePair<string, Type> pair in paragonKeys)
                {
                    if (paragonsEnabled)
                    {
                        paragons.Add(new Tuple<TowerModel, UpgradeModel>(
                            (TowerModel)pair.Value.GetField("towerModel").GetValue(null),
                            (UpgradeModel)pair.Value.GetField("upgradeModel").GetValue(null)
                            ));
                        Game.instance.model.AddUpgrade(paragons.Last().Item2);
                        Game.instance.model.AddTowerToGame(paragons.Last().Item1);
                        MelonLogger.Msg(pair.Key + " Paragon loaded!");
                    }
                }

                Game.instance.GetLocalizationManager().textTable["BananaFarm Paragon"] = "Banana Trade Center";
                Game.instance.GetLocalizationManager().textTable["BananaFarm Paragon Description"] =
                    "As more time is spent\nmore bananas are made.";

                Game.instance.GetLocalizationManager().textTable["EngineerMonkey Paragon"] = "Sentry Divos";
                Game.instance.GetLocalizationManager().textTable["EngineerMonkey Paragon Description"] =
                   "Pog";
            }
        }

        //! "unlocking" paragons
        [HarmonyPatch(typeof(ProfileModel), nameof(ProfileModel.Validate))]
        class ProfileModel_Validate
        {
            [HarmonyPostfix]
            internal static void Postfix(ProfileModel __instance)
            {
                foreach (var paragonPair in paragons)
                {
                    __instance.acquiredUpgrades.Add(paragonPair.Item2.name);
                    __instance.unlockedTowers.Add(paragonPair.Item1.name);
                }
            }
        }

        //! paragon testing
        [HarmonyPatch(typeof(Btd6Player), nameof(Btd6Player.CheckShowParagonPip))]
        class Btd6Player_CheckShowParagonPip
        {
            [HarmonyPrefix]
            internal static bool Prefix(string towerId)
            {
                bool isCustomParagon = false;
                foreach (var paragonPair in paragons)
                {
                    if (towerId == paragonPair.Item1.baseId)
                    {
                        isCustomParagon = true;
                    }
                }
                return !isCustomParagon;
            }
        }

        //! tiers for paragons
        [HarmonyPatch(typeof(GameModel), nameof(GameModel.CreateModded), new Type[] { typeof(GameModel), typeof(Il2CppSystem.Collections.Generic.List<ModModel>) })]
        class GameModel_CreateModded
        {
            [HarmonyPostfix]
            internal static void Postfix(GameModel result)
            {
                foreach (var paragonPair in paragons)
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

        //! BananaFarm degree & stat update
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
                        if (simTower.IsParagon && towerModel.baseId == "BananaFarm")
                        {
                            var cash = simTower.tower.cashEarned;
                            var paragonTower = simTower.tower.GetTowerBehavior<ParagonTower>();
                            var degree = paragonTower.GetCurrentDegree();
                            if (cash >= (degree * 30000) && degree < 100)
                            {
                                // Degree update
                                ParagonTower.InvestmentInfo info = paragonTower.investmentInfo;
                                info.totalInvestment = Game.instance.model.paragonDegreeDataModel.powerDegreeRequirements[degree];
                                paragonTower.investmentInfo = info;
                                paragonTower.UpdateDegree();
                            }
                            // Adjust income based on degree
                            var cashModel = towerModel.GetWeapon().projectile.GetBehavior<CashModel>();
                            var amount = 500 + (5f * degree) * (2 * (float)Math.Floor(new Decimal(degree / 10)) + 2);
                            if (cashModel.minimum != amount)
                            { cashModel.minimum = amount; cashModel.maximum = amount; }
                        }
                    }
                }
            }
        }

        //! Adjust paragon stats for degree
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
                            var towerModel = simTower.tower.towerModel;
                            var degree = simTower.tower.GetTowerBehavior<ParagonTower>().GetCurrentDegree();
                            //!Engineer
                            if (towerModel.baseId == "EngineerMonkey")
                            {
                                // TODO: Add paragon degree handling
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}