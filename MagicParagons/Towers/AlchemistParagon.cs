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
using MelonLoader;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using BTD_Mod_Helper.Api.Display;
using Assets.Scripts.Unity.Display;
using UnityEngine;
using MagicParagons.util;

namespace MagicParagons.Towers
{
    class AlchemistParagon : ModdedParagon
    {
        // Class info
        public static float Price = 300000;
        public static string BaseTower = ALCH;
        public static string TowerClass = MAGIC;
        // Paragon localization
        public static string DisplayName = "The Catalyst";
        public static string Description = "Alchemist description.";
        //
        public static TowerModel Tower;
        public static UpgradeModel Upgrade;
        //
        static AlchemistParagon()
        {
            List<TowerModel> Towers = new List<TowerModel>()
            {
                Game.instance.model.GetTower(BaseTower),
                Game.instance.model.GetTower(BaseTower,5),
                Game.instance.model.GetTower(BaseTower,0,5),
                Game.instance.model.GetTower(BaseTower,0,0,5)
            };
            setupTower(ref Upgrade, ref Tower, TowerClass, BaseTower, Price, Towers[1]);

            //! Custom Behavior

        }
    }
}
