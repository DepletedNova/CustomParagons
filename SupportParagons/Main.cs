using Assets.Main.Scenes;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Profile;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Simulation.Towers.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Bridge;
using Assets.Scripts.Unity.UI_New.InGame;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using MelonLoader;
using SupportParagons.Towers;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: MelonInfo(typeof(SupportParagons.Main), "Support Paragons", "1.2.0", "DepletedNova")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace SupportParagons
{
    public class Main : BloonsTD6Mod
    {
        //! Paragons
        static Dictionary<string, Type> paragonKeys = new Dictionary<string, Type>()
        {
            { "BananaFarm", typeof(FarmParagon) },
            { "EngineerMonkey", typeof(EngineerParagon) },
            { "SpikeFactory", typeof(SpikeParagon) },
            { "MonkeyVillage", typeof(VillageParagon) }
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
                   "Sentries and plasma make for a decent combination.";

                Game.instance.GetLocalizationManager().textTable["SpikeFactory Paragon"] = "Landmine Overseer";
                Game.instance.GetLocalizationManager().textTable["SpikeFactory Paragon Description"] =
                   "Don't tread on me.";

                Game.instance.GetLocalizationManager().textTable["MonkeyVillage Paragon"] = "Monkey Capital";
                Game.instance.GetLocalizationManager().textTable["MonkeyVillage Paragon Description"] =
                   "Incredible power secretes from this tower.";
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

        /*[HarmonyPatch(typeof(Btd6Player), nameof(Btd6Player.CheckForNewParagonPipEvent))]
        class Btd6Player_CheckShowParagonPip
        {
            [HarmonyPrefix]
            internal static ValueTuple<bool,bool> Prefix(string towerId, string checkSpecificTowerSet)
            {
                MelonLogger.Msg(towerId);
                MelonLogger.Msg(checkSpecificTowerSet);
                return new ValueTuple<bool, bool>(false, false);
            }
        }*/

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
                        if (simTower.IsParagon)
                        {
                            if (towerModel.baseId == "BananaFarm") { //! Banana Farm
                                var cash = simTower.tower.cashEarned;
                                var paragonTower = simTower.tower.GetTowerBehavior<ParagonTower>();
                                var degree = paragonTower.GetCurrentDegree();
                                float mathed = (float)Math.Floor(new Decimal(degree / 10));
                                if (cash >= (degree * 30000 + (mathed*10000)) && degree < 100)
                                {
                                    // Degree update
                                    ParagonTower.InvestmentInfo info = paragonTower.investmentInfo;
                                    info.totalInvestment = Game.instance.model.paragonDegreeDataModel.powerDegreeRequirements[degree];
                                    paragonTower.investmentInfo = info;
                                    paragonTower.UpdateDegree();
                                }
                                // Adjust income based on degree
                                var towerModelCash = towerModel.GetAttackModels()[1].weapons[0].projectile
                                    .GetBehavior<CreateTowerModel>().tower.GetWeapon().projectile.GetBehavior<CashModel>();
                                var cashModel = towerModel.GetWeapon().projectile.GetBehavior<CashModel>();
                                var amount = (5f * degree) * (2 * mathed + 2);
                                if (cashModel.minimum != 500 + amount)
                                {
                                    cashModel.minimum = 500 + amount; cashModel.maximum = 500 + amount;
                                    towerModelCash.minimum = 400 + amount; towerModelCash.maximum = 400 + amount;
                                }
                            }
                        }
                    }
                }
            }
        }

        //! Adjust paragon custom stats for degree
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
                            
                            if (towerModel.baseId == "EngineerMonkey") //! Engineer
                            {
                                var tTower = towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<CreateTowerModel>().tower;
                                float mathDegree = (float)Math.Floor(new Decimal(degree / 10));
                                tTower.GetAttackModels()[1].weapons[0].rate = .3f / (mathDegree + 1);
                                tTower.GetAttackModels()[1].weapons[0].projectile.pierce += 50 * (mathDegree + 1) + (5 * (degree - 1));
                                tTower.GetAttackModels()[1].weapons[0].projectile.GetDamageModel().damage = 15 * (mathDegree + 1) + (degree - 1);
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}