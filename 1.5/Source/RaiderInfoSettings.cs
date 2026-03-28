using UnityEngine;
using Verse;

namespace RaiderInfo;
public class RaiderInfoSettings : ModSettings {
	public bool alertForInsects = true;
	public bool alertForFogged = false;
	public bool alertForInvisible = false;

	public override void ExposeData() {
		base.ExposeData();

		Scribe_Values.Look(ref alertForInsects, "alertForInsects", true);
		Scribe_Values.Look(ref alertForFogged, "alertForFogged", false);
		Scribe_Values.Look(ref alertForInvisible, "alertForInvisible", false);
	}

	public void DoSettingsWindowContents(Rect inRect) {
		Listing_Standard listing_Standard = new Listing_Standard();
		inRect.SplitVerticallyWithMargin(out Rect left, out Rect _, 34);
		listing_Standard.Begin(left);

		listing_Standard.CheckboxLabeled("RaiderInfo.AlertForInsects".Translate(), ref alertForInsects);
		listing_Standard.CheckboxLabeled("RaiderInfo.AlertForFogged".Translate(), ref alertForFogged);
		listing_Standard.CheckboxLabeled("RaiderInfo.AlertForInvisible".Translate(), ref alertForInvisible);

		listing_Standard.End();
	}
}
