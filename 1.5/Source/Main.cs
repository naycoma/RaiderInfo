using System.Reflection;

using Verse;
using HarmonyLib;

namespace RaiderInfo;

[StaticConstructorOnStartup]
public static class Main {
    private const string HarmonyPatchID = "bluebird.com.tammybee.raiderinfo";
    static Main() => new Harmony(HarmonyPatchID).PatchAll(Assembly.GetExecutingAssembly());
}