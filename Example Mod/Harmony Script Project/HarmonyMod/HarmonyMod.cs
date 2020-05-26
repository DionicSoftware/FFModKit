using UnityEngine;
using System.Linq;
using HarmonyLib;


// this example file shows how to integrate use Harmony modding in Founders' Fortune
// if you're new to script modding, please refer to the other example script mod first to learn the basics!

namespace HarmonyMod {

    [System.Serializable]
    public class HarmonyMod : Mod {
        
        public override void Load() {
            // use name of your mod as ID to avoid mixups with other mods
            Harmony harmony = new Harmony("HarmonyMod");

            // applies all patches using annotations like [HarmonyPatch]
            harmony.PatchAll();
        }

        public override void Start() {
            
        }

        public override void Update() {

        }
    }


    [HarmonyPatch(typeof(HumanAI))]
    public class NamePatch {

        [HarmonyPatch("GetFullName")]
        public static bool Prefix(ref string __result) {
            __result = "Harmony-Patched Name!"; // replaces the return value of the original method
            return false; // skips the original method
        }
    }
}

