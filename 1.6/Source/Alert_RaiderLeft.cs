using System.Collections.Generic;
using System.Linq;
using System.Text;

using RimWorld;
using Verse;
using Verse.AI.Group;

namespace RaiderInfo;
public class Alert_RaiderLeft : Alert {
	private IEnumerable<Lord> RaiderLords {
		get {
			foreach (Map map in Find.Maps) {
				if (!map.IsPlayerHome) {
					continue;
				}
				foreach (Lord lord in map.lordManager.lords) {
					Faction faction = lord.faction;
					List<Pawn> ownedPawns = lord.ownedPawns;
					if (!faction.HostileTo(Faction.OfPlayer) || ownedPawns.NullOrEmpty()) {
						continue;
					}
					if (faction.def == FactionDefOf.Insect && !RaiderInfoMod.Settings.alertForInsects) {
						continue;
					}
					if (ownedPawns.All(p => p.mindState.Active) && ownedPawns.Any(IsVisible)) {
						yield return lord;
					}
				}
			}
		}
	}

	public Alert_RaiderLeft() {
		defaultLabel = "RaiderInfo.RaiderLeftLabel".Translate();
		defaultPriority = AlertPriority.High;
	}

	public override TaggedString GetExplanation() {
		if (this.RaiderLords == null) return "";

		StringBuilder stringBuilder = new StringBuilder();

		foreach (Lord current in this.RaiderLords) {
			if (stringBuilder.Length > 0) {
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(this.GetRaiderLeft(current));
		}
		return stringBuilder.ToString();
	}

	public string GetRaiderLeft(Lord lord) {
		string factionName = lord.faction.Name;
		int raiderLeft = CountRaider(lord);
		bool isHumanLike = lord.faction?.leader?.RaceProps?.Humanlike ?? false;
		string unitOfRaider = isHumanLike ? "RaiderInfo.RaiderLeftUnitOfHuman".Translate() : "RaiderInfo.RaiderLeftUnitOfNonHuman".Translate();

		return "RaiderInfo.RaiderLeft".Translate(factionName, raiderLeft, unitOfRaider);
	}

	public int CountRaider(Lord lord) {
		return lord.ownedPawns?.Count(p => IsStanding(p) && IsVisible(p)) ?? 0;
	}

	public bool IsStanding(Pawn p) {
		if (!p.Spawned || p.Dead) {
			return false;
		}
		return !p.Downed && !p.Deathresting;
	}
	public bool IsVisible(Pawn p) {
		if (p.Map == null) return false;

		if (!RaiderInfoMod.Settings.alertForFogged && p.Map.fogGrid.IsFogged(p.Position)) return false;

		if (RaiderInfoMod.Settings.alertForInvisible) return true;

		// if (p.IsPsychologicallyInvisible()) return false;

		return !p.IsHiddenFromPlayer();
	}

	public override AlertReport GetReport() {
		if (Find.AnyPlayerHomeMap == null) return false;

		if (RaiderLords.FirstOrDefault() == null) return false;

		List<Pawn> raiders = RaiderLords.SelectMany(l => l.ownedPawns.Where(IsVisible)).ToList();
		return AlertReport.CulpritsAre(raiders);
	}
}
