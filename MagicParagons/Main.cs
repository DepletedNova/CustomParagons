using Assets.Main.Scenes;
using BTD_Mod_Helper.Api.ModOptions;
using HarmonyLib;
using MelonLoader;
using System;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(MagicParagons.Main), "Magic Paragons", "1.0.0", "DepletedNova")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace MagicParagons
{
    class Main : ParagonMod
    {
        static Dictionary<string, Type> Paragons = new Dictionary<string, Type>()
        {

        };

        static List<ModSettingBool> paragonSettings;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            foreach (KeyValuePair<string, Type> pair in Paragons)
                paragonSettings.Add(new ModSettingBool(true) { displayName = $"Enable {pair.Key} Paragon? (Requires restart.)" });
            MelonLogger.Msg("Settings loaded.");
        }
    }
}
