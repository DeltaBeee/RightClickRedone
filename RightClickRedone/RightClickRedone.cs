using BepInEx;
using UnboundLib;
using UnboundLib.Cards;
using RightClickRedone.Cards;
using HarmonyLib;
using CardChoiceSpawnUniqueCardPatch.CustomCategories;


namespace RightClickRedone {
    [BepInDependency("com.willis.rounds.unbound", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.moddingutils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pykess.rounds.plugins.cardchoicespawnuniquecardpatch", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RCRedone : BaseUnityPlugin {

        private const string ModId = "com.delta.rounds.RCRedone";
        private const string ModName = "Right Click Redone";
        public const string Version = "0.1.0";
        public const string ModInitials = "RCR";
        public static RCRedone instance {get;private set;}

        void Awake() {
            // Use this to call any harmony patch files your mod may have
            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }
        void Start() {
            instance = this;
            CustomCard.BuildCard<MyCardName>();
        }
    }
}
