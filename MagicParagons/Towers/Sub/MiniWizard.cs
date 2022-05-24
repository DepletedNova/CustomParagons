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
using System.Collections.Generic;
using UnhollowerBaseLib;

namespace MagicParagons.Towers.Sub
{
    class MiniWizard : ModTower
    {
        public override string TowerSet => MAGIC;
        public override string BaseTower => "WizardMonkey-502"; 
        public override string Name => "GhostWizard";

        public override string DisplayName => "Ghastly Wizard";
        public override string Description => "";
        public override int Cost => 0;

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override bool DontAddToShop => true;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            // Base Tower behavior
            Tower.range = 80;

            // Sub-Tower behavior
            Tower.RemoveBehavior<CreateSoundOnTowerPlaceModel>();
            Tower.AddBehavior(new CreditPopsToParentTowerModel("CreditPops"));
            Tower.AddBehavior(new TowerExpireModel("GhostWizard", 12,999, false, false));
            Tower.isSubTower = true;
            Tower.footprint = Game.instance.model.GetTower(TowerType.Sentry).footprint.Duplicate();

            // Attack behavior
            Tower.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("MOAB", "Moabs", 1, 20, false, true));
            Tower.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 2.5f, 0, false, true));
            Tower.GetAttackModel().weapons[0].projectile.GetDamageModel().damage = 40;
            Tower.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            Tower.GetAttackModel().weapons[0].projectile.pierce = 40;
            Tower.GetAttackModel().weapons[0].rate = 0.15f;
            Tower.GetAttackModel().range = 80;

        }
    }
}
