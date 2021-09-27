using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;

namespace SupportParagons.Towers.Sub
{
    class OverclockedSentry : ModTower
    {
        public override string TowerSet => SUPPORT;
        public override string BaseTower => TowerType.SentryParagon;
        public override int Cost => 0;

        public override string DisplayName => "Overclocked Sentry";
        public override string Description => "Unformidable.";

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            var towerExpireModel = Tower.GetBehavior<TowerExpireModel>();
            towerExpireModel.lifespan = 7.5f;
            Tower.range = 80; Tower.GetAttackModel().range = 80;
            Tower.GetWeapon().projectile.GetDamageModel().damage = 100;
            Tower.GetWeapon().rate = 0.1f;

            Tower.AddBehavior(new OverrideCamoDetectionModel("OCDM_OverSentry", true));
        }

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return -1;
        }
    }
}
