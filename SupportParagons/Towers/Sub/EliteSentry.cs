using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.TowerSets;
using Assets.Scripts.Unity;
using Assets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using System.Collections.Generic;

namespace SupportParagons.Towers.Sub
{
    class EliteSentry : ModTower
    {
        public override string TowerSet => SUPPORT;
        public override string BaseTower => TowerType.Sentry;
        public override int Cost => 0;

        public override string DisplayName => "Elite Sentry";
        public override string Description => "Impoppable.";

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            var towerExpireModel = Tower.GetBehavior<TowerExpireModel>();
            towerExpireModel.lifespan = 20f;
            Tower.range = 40; Tower.isGlobalRange = true;
            Tower.GetWeapon().projectile.AddBehavior(new InstantModel("EliteSentry_Instant",true));
            Tower.GetWeapon().projectile.GetDamageModel().damage = 5000;
            Tower.GetWeapon().projectile.GetDamageModel().distributeToChildren = true;
            Tower.GetWeapon().rate = 99f;
            Tower.GetAttackModel().range = 2000;

            Tower.AddBehavior(new OverrideCamoDetectionModel("OCDM_OverSentry", true));
        }

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return -1;
        }
    }

    class EliteSentryDisplay : ModTowerDisplay<EliteSentry>
    {
        public override string BaseDisplay => GetDisplay(TowerType.Sentry);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            var basic = node.gameObject.transform.FindChild("LOD_1");
            var v3 = new UnityEngine.Vector3(1.1f, 1.1f, 1.1f);

            basic.set_localScale_Injected(value: ref v3);
            node.Scale = v3;
        }

        public override bool UseForTower(int[] tiers)
        {
            return true;
        }
    }
}
