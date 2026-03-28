using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using HarmonyLib;

namespace RaiderInfo;
public class Alert_MechAssembler : Alert {
	private IEnumerable<Thing> MechAssemblers {
		get {
			if (!ModsConfig.RoyaltyActive) {
				yield break;
			}
			foreach (Map map in Find.Maps) {
				foreach (Thing mechAssembler in map.listerThings.ThingsOfDef(ThingDef.Named("MechAssembler"))) {
					yield return mechAssembler;
				}
			}
		}
	}

	private List<Thing> MechAssemblersOrderByNextSpawnTick {
		get {
			return this.MechAssemblers?
			.Where(t => t.TryGetComp<CompSpawnerPawn>()?.Active ?? false)
			.OrderBy(t => t.TryGetComp<CompSpawnerPawn>()?.nextPawnSpawnTick ?? -1)
			.ToList();
		}
	}

	public Alert_MechAssembler() {
		this.defaultPriority = AlertPriority.High;
	}

	public override string GetLabel() {
		List<Thing> ordered = this.MechAssemblersOrderByNextSpawnTick;
		if (ordered.NullOrEmpty()) return "";

		CompSpawnerPawn comp = ordered.First().TryGetComp<CompSpawnerPawn>();
		int leftTick = comp.nextPawnSpawnTick - Find.TickManager.TicksGame;

		return "RaiderInfo.MechAssemblerLabel".Translate(leftTick.ToStringTicksToPeriod());
	}

	public override TaggedString GetExplanation() {
		if (this.MechAssemblers == null) return "RaiderInfo.MechAssemblerDescBase".Translate("");

		StringBuilder stringBuilder = new StringBuilder();
		foreach (Thing mechAssembler in this.MechAssemblersOrderByNextSpawnTick) {
			CompSpawnerPawn comp = mechAssembler.TryGetComp<CompSpawnerPawn>();
			if (comp == null) continue;

			int leftTick = comp.nextPawnSpawnTick - Find.TickManager.TicksGame;

			PawnKindDef chosenKind = Traverse.Create(comp).Field("chosenKind").GetValue<PawnKindDef>();

			if (stringBuilder.Length > 0) stringBuilder.AppendLine();
			stringBuilder.Append("RaiderInfo.MechAssemblerDescItem".Translate(chosenKind.LabelCap, leftTick.ToStringTicksToPeriod()));
		}
		return "RaiderInfo.MechAssemblerDescBase".Translate(stringBuilder.ToString());
	}

	public override AlertReport GetReport() {
		if (!ModsConfig.RoyaltyActive) return false;

		if (Find.AnyPlayerHomeMap == null) return false;

		if (this.MechAssemblers == null) return false;

		return AlertReport.CulpritsAre(this.MechAssemblersOrderByNextSpawnTick);
	}
}
