using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Upgrades;
using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Mods;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Weapons;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Unity;
using Assets.Scripts.Utils;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using BTD_Mod_Helper.Api.Display;
using Assets.Scripts.Unity.Display;
using UnityEngine;
using MagicParagons.util;
using Assets.Scripts.Models.Towers.Behaviors.Emissions;
using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using MelonLoader;

namespace MagicParagons.Towers
{
    class WizardParagon : ModdedParagon
    {
        // Class info
        public static float Price = 300000;
        public static string BaseTower = WIZARD;
        public static string TowerClass = MAGIC;
        // Paragon localization
        public static string DisplayName = "Hell's Harbringer";
        public static string Description = "Wizard description.";
        //
        public static TowerModel Tower;
        public static UpgradeModel Upgrade;
        //
        static WizardParagon()
        {
            List<TowerModel> Towers = new List<TowerModel>()
            {
                Game.instance.model.GetTower(BaseTower),
                Game.instance.model.GetTower(BaseTower,5),
                Game.instance.model.GetTower(BaseTower,0,5),
                Game.instance.model.GetTower(BaseTower,0,0,5)
            };
            setupTower(ref Upgrade, ref Tower, TowerClass, BaseTower, Price, Towers[3]);

            //! Custom Behavior

            Tower.range = 60;
            //var sauda = Game.instance.model.GetTowerFromId("Sauda 20");
            foreach (var x in Towers[3].GetAttackModels()[2].behaviors)
                MelonLogger.Msg(x.name);
        }
    }
}
