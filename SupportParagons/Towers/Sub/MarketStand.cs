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
    class MarketStand : ModTower
    {
        public override string TowerSet => SUPPORT;
        public override string BaseTower => "BananaFarm";
        public override int Cost => 0;

        public override string DisplayName => "Market Stall";
        public override string Description => "Money galore.";

        public override int TopPathUpgrades => 0;
        public override int MiddlePathUpgrades => 0;
        public override int BottomPathUpgrades => 0;

        public override void ModifyBaseTowerModel(TowerModel Tower)
        {
            Tower.RemoveBehavior<CreateSoundOnTowerPlaceModel>();

            Tower.RemoveBehavior<AttackModel>();
            var f500 = Game.instance.model.GetTower("BananaFarm", 4);
            Tower.AddBehavior(f500.GetAttackModel().Duplicate());

            var expireModel = new TowerExpireModel("FarmParagon_SubTower_Expire", 10f,999, false, false);
            Tower.AddBehavior(expireModel);
            Tower.isSubTower = true;

            Tower.footprint = Game.instance.model.GetTower(TowerType.Sentry).footprint.Duplicate();
        }

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return -1;
        }
    }

    class MarketStandDisplay : ModTowerDisplay<MarketStand>
    {
        public override string BaseDisplay => GetDisplay("BananaFarm", 0, 0, 3);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            var basic = node.gameObject.transform.FindChild("LOD_1");
            var v3 = new UnityEngine.Vector3(0.75f, 0.75f, 0.75f);

            basic.set_localScale_Injected(value: ref v3);
            node.Scale = v3;
        }

        public override bool UseForTower(int[] tiers)
        {
            return true;
        }
    }
}