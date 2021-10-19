using Assets.Scripts.Models;
using Assets.Scripts.Models.Bloons.Behaviors;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Models.Towers.Projectiles;
using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
using Assets.Scripts.Models.Towers.Upgrades;
using Assets.Scripts.Models.Towers.Weapons.Behaviors;
using Assets.Scripts.Unity;
using BTD_Mod_Helper.Extensions;
using MagicParagons.util;
using System.Collections.Generic;
using System.Linq;
using UnhollowerBaseLib;

namespace MagicParagons.Towers
{
    class DruidParagon : ModdedParagon
    {
        // Class info
        public static float Price = 600000;
        public static string BaseTower = DRUID;
        public static string TowerClass = MAGIC;
        // Paragon localization
        public static string DisplayName = "Nature's Calamity";
        public static string Description = "Druid description.";
        //
        public static TowerModel Tower;
        public static UpgradeModel Upgrade;
        //
        static DruidParagon()
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
            Tower.range = 80;
            // Red Vines
            var baseZone = Towers[2].GetBehavior<SpiritOfTheForestModel>().Duplicate();
            var DOTfar = new DamageOverTimeCustomModel("DOT_Far", 8, 1f, BloonProperties.None, null,
                0, true, new Il2CppStringArray(0), 1, 0, 0, false, 0, false, true, new Il2CppReferenceArray<DamageModifierModel>(0));
            var DOTmid = new DamageOverTimeCustomModel("DOT_Middle", 15, 1f, BloonProperties.None, null,
                0, true, new Il2CppStringArray(0), 1, 0, 0, false, 0, false, true, new Il2CppReferenceArray<DamageModifierModel>(0));
            var DOTclo = new DamageOverTimeCustomModel("DOT_Close", 30, 1f, BloonProperties.None, null,
                0, true, new Il2CppStringArray(0), 1, 0, 0, false, 0, false, true, new Il2CppReferenceArray<DamageModifierModel>(0));
            var DOTZfar = new DamageOverTimeZoneModel(DOTfar, "DOTZ_Far", 0, false, false, true, "DruidParagonFar");
            var DOTZmid = new DamageOverTimeZoneModel(DOTmid, "DOTZ_Middle", 0, false, false, true, "DruidParagonMiddle");
            var DOTZclo = new DamageOverTimeZoneModel(DOTclo, "DOTZ_Close", 0, false, false, true, "DruidParagonClose");
            baseZone.damageOverTimeZoneModelFar = DOTZfar; baseZone.damageOverTimeZoneModelMiddle = DOTZmid;
            baseZone.damageOverTimeZoneModelClose = DOTZclo; baseZone.middleRange = 175; baseZone.closeRange = 100;
            Tower.AddBehavior(baseZone);
            //Red Vine Slowdown
            var vineAttackModel = Game.instance.model.GetTower(GLUE,0,0,3).GetAttackModel().Duplicate();
            vineAttackModel.behaviors = vineAttackModel.behaviors
                .RemoveItemOfType<Model, TargetFirstModel>()
                .RemoveItemOfType<Model, TargetCloseModel>()
                .RemoveItemOfType<Model, TargetLastModel>();
            vineAttackModel.GetBehavior<TargetStrongModel>().isSelectable = false;
            vineAttackModel.targetProvider = vineAttackModel.GetBehavior<TargetStrongModel>();
            vineAttackModel.weapons[0].rate = 7.5f; vineAttackModel.range = 80;
            var vineProjectile = vineAttackModel.weapons[0].projectile;
            vineProjectile.GetBehavior<TravelStraitModel>().Speed = 100;
            vineProjectile.GetBehavior<TravelStraitModel>().lifespan = 0.65f;
            vineProjectile.GetBehavior<SlowModel>().mutationId = "druidParagonVine";
            vineProjectile.GetBehavior<SlowModel>().Multiplier = 0.4f;
            vineProjectile.GetBehavior<SlowModel>().Lifespan = 60;
            vineProjectile.GetBehavior<SlowModel>().dontRefreshDuration = true;
            vineProjectile.AddBehavior(new CollideOnlyWithTargetModel("TargetOnly"));
            var filters = new Il2CppReferenceArray<FilterModel>(new FilterModel[]
            {
                new FilterWithTagModel("MoabFilter","Moabs",false),
                new FilterOutTagModel("FilterBad","Bad",new Il2CppStringArray(0)),
                new FilterInvisibleModel("InvisFilter",true,false)
            });
            vineProjectile.GetBehavior<ProjectileFilterModel>().filters = filters;
            vineAttackModel.GetBehavior<AttackFilterModel>().filters = filters;
            Tower.AddBehavior(vineAttackModel);
            // Base attack
            Tower.AddBehavior(Towers[3].GetAttackModel().Duplicate());
            Tower.GetAttackModels()[1].GetBehavior<AttackFilterModel>().filters = new Il2CppReferenceArray<FilterModel>(
                new FilterModel[] {new FilterInvisibleModel("InvisFilter",false,false)});
            Tower.GetAttackModels()[1].weapons[0].projectile.GetDamageModel().damage = 60;
            Tower.GetAttackModels()[1].weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
            Tower.GetAttackModels()[1].weapons[0].projectile.SetHitCamo(true);
            Tower.GetAttackModels()[1].weapons[0].projectile.pierce = 100;
            Tower.GetAttackModels()[1].weapons[0].projectile.AddBehavior
                (new DamageModifierForTagModel("BossAdd", "Boss", 2, 0, false, true));
            Tower.GetAttackModels()[1].weapons[0].AddBehavior(new LeakDangerAttackSpeedModel("LeakDanger", 1.25f));
            Tower.GetAttackModels()[1].range = 80;
            Tower.AddBehavior(new StartOfRoundRateBuffModel("StartBuff", 2, 10));
            // Tornado
            var tornadoWeapon = Towers[1].GetAttackModel().weapons.First(a => a.name.Contains("Super")).Duplicate();
            tornadoWeapon.rate = 10;
            tornadoWeapon.projectile.RemoveBehaviors<RemoveMutatorsFromBloonModel>();
            Tower.GetAttackModels()[1].AddBehavior(tornadoWeapon);
            //? Stone mound bloon blocker
        }
    }
}
