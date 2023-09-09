using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace CatalystNoTierLock;

internal sealed class ClientConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[ReloadRequired]
	[DefaultValue(false)]
	public bool DisableIcon;
}