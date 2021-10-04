using Assets.Scripts.Models.GenericBehaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
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
using CreateEffectOnExpireModel = Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel;

namespace SupportParagons.Towers.Sub
{
    class OverclockedSentry : ModTower
    {
        public override string TowerSet => SUPPORT;
        public override string BaseTower => "SentryParagon";
        public override int Cost => 0;

        public override string DisplayName => "Overclocked Sentry";
        public override string Description => "Uigilias.";

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            var cpotd = Tower.GetBehavior<CreateProjectileOnTowerDestroyModel>();
            var projectile = cpotd.projectileModel.GetBehavior<CreateProjectileOnExpireModel>().projectile.Duplicate();
            projectile.radius = 40;
            projectile.filters = new UnhollowerBaseLib.Il2CppReferenceArray<FilterModel>(0);
            projectile.GetDamageModel().damage = 500; projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            projectile.SetHitCamo(true); projectile.pierce = 999;
            projectile.AddBehavior(cpotd.projectileModel.GetBehavior<CreateEffectOnExpireModel>().Duplicate());
            Tower.AddBehavior(Game.instance.model.GetTower("TackShooter").GetAttackModel().Duplicate());
            Tower.GetAttackModels()[1].weapons[0].projectile = projectile;
            Tower.GetAttackModels()[1].weapons[0].rate = 5;Tower.GetAttackModel().weapons[0].fireWithoutTarget = true;
            Tower.GetAttackModels()[1].weapons[0].projectile
                .AddBehavior(new DamageModifierForTagModel("OverclockedSentry_Bloonarius", "Boss", 2, 0, false, true));

            Tower.GetAttackModel().range = 80; Tower.range = 80;

            var nproj = Tower.GetAttackModel().weapons[0].projectile;
            nproj.pierce = 150; nproj.SetHitCamo(true);
            nproj.GetDamageModel().damage = 15; nproj.GetDamageModel().immuneBloonProperties = BloonProperties.None;

            Tower.GetBehavior<TowerExpireModel>().lifespan = 80;
            Tower.AddBehavior(new OverrideCamoDetectionModel("OverrideCamo_OverclockedSentry", true));
        }

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return -1;
        }
    }
}