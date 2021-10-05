using Assets.Scripts.Models.Towers;
using Assets.Scripts.Models.Towers.Upgrades;

namespace MagicParagons.Towers
{
    class WizardParagon : ModdedParagon
    {
        // Class info
        class WizardInfo : ModParagonInfo
        {
            public override float Price => 300000;
            public override string BaseTower => WIZARD;
            public override string TowerClass => MAGIC;
        }
        static WizardInfo Info = new WizardInfo();
        // Paragon localization
        public static string DisplayName = "Wizard Paragon";
        public static string Description = "Wizard descriptiion.";
        //
        public static TowerModel Tower;
        public static UpgradeModel Upgrade;
        //
        static WizardParagon()
        {
            setupTower(ref Upgrade, ref Tower, Info.TowerClass, Info.BaseTower, Info.Price, Info.Icon, Info.Display);

            //! Custom Behavior

        }
    }
}
