using CatalystMod;
using CatalystMod.Items.SummonItems;
using CatalystMod.NPCs;
using CatalystMod.UI;
using MonoMod.RuntimeDetour.HookGen;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CatalystNoTierLock;

public sealed class CatalystNoTierLock : Mod
{
	private delegate bool orig_CommunicatorCanUseItem(AstralCommunicator self, Player player);

	private delegate void orig_CommunicatorTooltip(AstralCommunicator self, List<TooltipLine> tooltips);

	private delegate void orig_PlayerEquips(CatalystPlayer self);
	
	private delegate void orig_PlayerLoadData(CatalystPlayer self, TagCompound tag);

	private delegate bool orig_NPCPreKill(CatalystNPC self, NPC npc);

	private static readonly MethodBase _communicatorCanUseItem = typeof(AstralCommunicator).GetMethod(nameof(ModItem.CanUseItem));
	private static readonly MethodBase _communicatorTooltip = typeof(AstralCommunicator).GetMethod(nameof(ModItem.ModifyTooltips));
	private static readonly MethodBase _playerUpdateEquips = typeof(CatalystPlayer).GetMethod(nameof(ModPlayer.PostUpdateEquips));
	private static readonly MethodBase _playerLoadData = typeof(CatalystPlayer).GetMethod(nameof(ModPlayer.LoadData));
	private static readonly MethodBase _npcPreKill = typeof(CatalystNPC).GetMethod(nameof(GlobalNPC.PreKill));

	private static readonly PropertyInfo _summonAstrageldonFlag = typeof(CatalystPlayer).GetProperty("SummonAstrageldon", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

	public override void Load()
	{
		HookEndpointManager.Add(_communicatorCanUseItem, CanUseCheck);
		HookEndpointManager.Add(_communicatorTooltip, TooltipCheck);
		HookEndpointManager.Add(_playerUpdateEquips, EquipsCheck);
		HookEndpointManager.Add(_playerLoadData, ForceData);
		HookEndpointManager.Add(_npcPreKill, PreKillCheck);

		if (ModContent.GetInstance<ClientConfig>().DisableIcon)
		{
			LoadMenu.PlayerLoadIcons.RemoveAll(icon => icon.hoverNameKey == "Mods.CatalystMod.TitleIcon.Astrageldon");
		}
	}

	private static bool CanUseCheck(orig_CommunicatorCanUseItem orig, AstralCommunicator self, Player player)
	{
		bool originalValue = NPC.downedMoonlord;
		NPC.downedMoonlord = false;
		bool returned = orig(self, player);
		NPC.downedMoonlord = originalValue;
		return returned;
	}

	private static void TooltipCheck(orig_CommunicatorTooltip orig, AstralCommunicator self, List<TooltipLine> tooltips)
	{
		bool originalValue = NPC.downedMoonlord;
		NPC.downedMoonlord = false;
		orig(self, tooltips);
		NPC.downedMoonlord = originalValue;
	}

	private static void EquipsCheck(orig_PlayerEquips orig, CatalystPlayer self)
	{
		bool originalValue = NPC.downedMoonlord;
		NPC.downedMoonlord = false;
		orig(self);
		NPC.downedMoonlord = originalValue;
	}

	private static void ForceData(orig_PlayerLoadData orig, CatalystPlayer self, TagCompound tag)
	{
		orig(self, tag);
		_summonAstrageldonFlag.GetSetMethod(true).Invoke(self, new object[] { true });
	}

	private static bool PreKillCheck(orig_NPCPreKill orig, CatalystNPC self, NPC npc)
	{
		bool originalValue = NPC.downedMoonlord;
		NPC.downedMoonlord = true; // Message only shows if this is false, so set to true to stop the message.
		bool returned = orig(self, npc);
		NPC.downedMoonlord = originalValue;
		return returned;
	}
}