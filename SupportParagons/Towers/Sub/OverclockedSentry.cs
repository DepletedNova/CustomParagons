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
            towerExpireModel.lifespan = 6.75f;
        }

        public override int GetTowerIndex(List<TowerDetailsModel> towerSet)
        {
            return -1;
        }
    }

    class OverclockedSentryDisplay : ModTowerDisplay<OverclockedSentry>
    {
        public override string BaseDisplay => GetDisplay(TowerType.SentryParagon);

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            node.PrintInfo();
            node.SaveMeshTexture();
        }

        public override bool UseForTower(int[] tiers)
        {
            return true;
        }
    }
}
