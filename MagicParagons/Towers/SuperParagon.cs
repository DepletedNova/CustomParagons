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
    class SuperParagon : ModdedParagon
    {
        // Class info
        public static float Price = 300000;
        public static string BaseTower = SUPER;
        public static string TowerClass = MAGIC;
        // Paragon localization
        public static string DisplayName = "Super Paragon";
        public static string Description = "Super description.";
        //
        public static TowerModel Tower;
        public static UpgradeModel Upgrade;
        //
        static SuperParagon()
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
            Tower.display = "f5cba0f9752b01545960aef3e3a8d06d";
            Tower.GetBehavior<DisplayModel>().display = "f5cba0f9752b01545960aef3e3a8d06d";

            Tower.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = "f5cba0f9752b01545960aef3e3a8d06d");
            /*foreach (var x in Towers[1].behaviors)
                MelonLogger.Msg(x.name);*/
        }
    }

    public class YellowBeam : ModDisplay
    {
        public override string BaseDisplay => "b9f3014db2da83f48b34e662e9a79910";
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (var renderer in node.genericRenderers)
            {
                var meshRenderer = renderer.Cast<MeshRenderer>();
                meshRenderer.material.SetColor("_Color1", Color.yellow);
                meshRenderer.material.SetColor("_Color2", Color.yellow);
            }
        }
        public override string Name => "YellowBeam";
    }
}
