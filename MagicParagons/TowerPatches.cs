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
using Assets.Scripts.Simulation.Towers.Mutators;
using Assets.Scripts.Models.Towers.Mutators;
using Assets.Scripts.Simulation.Input;
using Assets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Assets.Scripts.Unity.UI_New.InGame.StoreMenu;

namespace MagicParagons.Patches
{
    [HarmonyPatch(typeof(UnityToSimulation), nameof(UnityToSimulation.UpgradeTowerParagon))]
    class UpgradeParagon_PhoenixPatch
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
                        //! Wizard Paragon phoenix fix
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
}
