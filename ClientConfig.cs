using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CatalystNoTierLock;

internal sealed class ClientConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[ReloadRequired]
	[DefaultValue(false)]
	[Label($"$Mods.{nameof(CatalystNoTierLock)}.Config.{nameof(DisableIcon)}.Label")]
	[Tooltip($"$Mods.{nameof(CatalystNoTierLock)}.Config.{nameof(DisableIcon)}.Tooltip")]
	public bool DisableIcon;
}