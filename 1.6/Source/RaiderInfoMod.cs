using UnityEngine;
using Verse;

namespace RaiderInfo;
public class RaiderInfoMod : Mod {
	public static RaiderInfoSettings Settings {
		get {
			return LoadedModManager.GetMod<RaiderInfoMod>().settings;
		}
	}

	public RaiderInfoSettings settings;

	public RaiderInfoMod(ModContentPack content) : base(content) {
		this.settings = GetSettings<RaiderInfoSettings>();
	}

	public override void DoSettingsWindowContents(Rect inRect) {
		Settings.DoSettingsWindowContents(inRect);
	}

	public override string SettingsCategory() {
		return "Raider Info";
	}
}
