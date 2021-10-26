using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using MelonLoader;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;

namespace MagicParagons.Towers.Sub
{
    class ParagonPhoenix : ModTower
    {
        public override string Name => "ParagonPheonix";
        public override string TowerSet => MAGIC;
        public override string BaseTower => "PermaPhoenix";
        public override int Cost => 0;

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override string DisplayName => "Yin-Yang";
        public override string Description => "";

        public override bool DontAddToShop => true;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            var weaponModel = Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].Duplicate();
            Tower.GetAttackModel().weapons[0] = weaponModel;
            weaponModel.animateOnMainAttack = false;
            weaponModel.animation = 0;
        }
    }

    class ParagonPhoenixDisplay : ModTowerDisplay<ParagonPhoenix>
    {
        public override string BaseDisplay => "118269554079f3a4b8183c6e62d76d01";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            
        }

        public override bool UseForTower(int[] tiers)
        {
            return true;
        }
    }
}
