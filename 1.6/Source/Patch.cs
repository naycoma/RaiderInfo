using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;
using HarmonyLib;

namespace RaiderInfo;

//Verse.AI.Group.Lord SetJob
[HarmonyPatch(typeof(IncidentWorker_RaidEnemy))]
[HarmonyPatch(nameof(IncidentWorker_RaidEnemy.GetLetterText))]
class Patch_IncidentWorker_RaidEnemy_GetLetterText {
    [HarmonyPostfix]
    static void AppendCountOfRaiders(ref string __result, IncidentParms parms, List<Pawn> pawns) {
        if (parms.faction.def == FactionDefOf.Insect && !RaiderInfoMod.Settings.alertForInsects) {
            return;
        }
        int count = RaiderInfoMod.Settings.alertForInvisible ? pawns.Count : pawns.Count(p => !p.IsHiddenFromPlayer());
        if (count == 0) return;

        bool isHumanLike = parms.faction?.leader?.RaceProps?.Humanlike ?? false;
        string unitOfRaider = isHumanLike ? "RaiderInfo.RaiderLeftUnitOfHuman".Translate() : "RaiderInfo.RaiderLeftUnitOfNonHuman".Translate();

        __result += "\n\n";
        __result += "RaiderInfo.CountOfRaiders".Translate(count, unitOfRaider);
    }
}

[HarmonyPatch(typeof(Alert))]
[HarmonyPatch(nameof(Alert.DrawAt))]
class Patch_Alert_DrawAt {
    public static bool inDrawAt = false;

    private static void Prefix() {
        inDrawAt = true;
    }

    private static void Postfix() {
        inDrawAt = false;
    }
}

[HarmonyPatch(typeof(CameraJumper))]
[HarmonyPatch(nameof(CameraJumper.TryJumpAndSelect))]
internal class Patch_CameraJumper_TryJumpAndSelect {
    public static int count;

    private static void Prefix() {
        if (Patch_Alert_DrawAt.inDrawAt) {
            count++;
        }
    }
}
