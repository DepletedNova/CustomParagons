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
using MagicParagons.Towers.Sub;

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

            // Display
            var wizardDisplay = ModContent.GetInstance<WizardDisplay>().Id;
            Tower.display = wizardDisplay;
            Tower.GetBehavior<DisplayModel>().display = wizardDisplay;
            Tower.GetBehavior<ParagonTowerModel>().displayDegreePaths.ForEach(path => path.assetPath = wizardDisplay);

            //! Custom Behavior
            Tower.range = 60;
            // Ghost creator
            var e100 = Game.instance.model.GetTower("EngineerMonkey", 1);
            Tower.AddBehavior(e100.GetAttackModel().Duplicate());
            var createAttackModel = Tower.GetAttackModel();
            createAttackModel.range = 60;
            createAttackModel.RemoveBehavior<RotateToTargetModel>();
            createAttackModel.RemoveBehavior<CreateEffectWhileAttackingModel>();
            createAttackModel.weapons[0].projectile.GetBehavior<DisplayModel>().display = null;
            createAttackModel.weapons[0].projectile.display = null;
            createAttackModel.weapons[0].projectile.GetBehavior<CreateTowerModel>().tower = ModContent.GetTowerModel<MiniWizard>().Duplicate();
            createAttackModel.weapons[0].rate = 3.75f;
            createAttackModel.weapons[0].animation = 0;
        }
    }

    public class WizardDisplay : ModDisplay
    {
        public override string BaseDisplay => "7dc46f26af35f39449aa94b70c3a53a1";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            var basic = node.gameObject.transform.FindChild("LOD_1");
            var v3 = new Vector3(1.4f, 1.4f, 1.4f);
            basic.set_localScale_Injected(ref v3);
            node.Scale = v3;
        }
    }
}
