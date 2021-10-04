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

namespace MagicParagons
{
    class ParagonHelper
    {
       public static void startPostFix(Dictionary<string, Type> Paragons,)
        {

        }
    }

    abstract class ParagonMod : BloonsTD6Mod
    {
        
    }

    abstract class ParagonTower
    {
        protected const string PRIMARY = "Primary";
        protected const string MILITARY = "Military";
        protected const string MAGIC = "Magic";
        protected const string SUPPORT = "Support";

        public abstract float Price { get; }
        public abstract string BaseTower { get; }
        public abstract string TowerType { get; }

        protected static TowerModel towerModel;
        protected static UpgradeModel upgradeModel;

        public abstract void baseBehavior();
        public virtual void UpdateAdjust() {}
    }
}
