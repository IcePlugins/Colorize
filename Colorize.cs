using Rocket.API;
using System.Collections.Generic;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace ExtraConcentratedJuice.Colorize
{
    public class Colorize : RocketPlugin<ColorizeConfig>
    {
        public Dictionary<string, Color> playerColors;
        public static Colorize instance;
        protected override void Load()
        {
            instance = this;
            Rocket.Core.Logging.Logger.Log("-----------------");
            Rocket.Core.Logging.Logger.Log("Extra's Colorizer");
            Rocket.Core.Logging.Logger.Log("-----------------");
            Rocket.Core.Logging.Logger.Log("> Blacklisted Colors:");
            foreach(string c in Configuration.Instance.banned_colors)
            {
                Rocket.Core.Logging.Logger.Log(c);
            }
            Rocket.Core.Logging.Logger.Log("> Enable Blacklist Bypass w/ Permission:" + Configuration.Instance.enable_bypass_permission);
            Rocket.Core.Logging.Logger.Log("   Reminder that the bypass permission is  colorizer.bypass");

            playerColors = new Dictionary<string, Color>();
            UnturnedPlayerEvents.OnPlayerChatted += OnChat;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerChatted -= OnChat;
        }

        private void OnChat(UnturnedPlayer player, ref Color color, string msg, EChatMode chatMode, ref bool cancel)
        {
            if (msg.StartsWith("/"))
            {
                return;
            }

            if (playerColors.TryGetValue(player.Id, out Color playerColor) && player.HasPermission("colorize"))
            {
                cancel = true;
                SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(player.CSteamID);

                if (chatMode == EChatMode.GLOBAL)
                {
                    ChatManager.instance.channel.send("tellChat", ESteamCall.OTHERS, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                {
                            player.CSteamID,
                            (byte)chatMode,
                            playerColor,
                            msg
                });
                }
                else if (chatMode == EChatMode.LOCAL)
                {
                    ChatManager.instance.channel.send("tellChat", ESteamCall.OTHERS, steamPlayer.player.transform.position, EffectManager.MEDIUM, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                            {
                            player.CSteamID,
                            (byte)chatMode,
                            playerColor,
                            msg
                            });
                }
                else if (chatMode == EChatMode.GROUP && player.SteamGroupID != CSteamID.Nil)
                {
                    for (int i = 0; i < Provider.clients.Count; i++)
                    {
                        SteamPlayer otherPlayer = Provider.clients[i];
                        if (otherPlayer != null && player.SteamGroupID == UnturnedPlayer.FromSteamPlayer(otherPlayer).SteamGroupID)
                        {
                            ChatManager.instance.channel.send("tellChat", otherPlayer.playerID.steamID, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[]
                            {
                                    player.CSteamID,
                                    (byte)chatMode,
                                    playerColor,
                                    msg
                            });
                        }
                    }
                }
            }
        }
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"colorize_invalid_args", "[Colorize] Invalid arguments. Correct usage: /colorize <color or #XXXXXX hexcode>"},
                    {"colorize_invalid_color", "[Colorize] Invalid color. Correct usage: /colorize <color or #XXXXXX hexcode>"},
                    {"colorize_success", "[Colorize] Success! Your color has been set to #{0}."},
                    {"colorize_blacklisted", "[Colorize] You cannot set that color (#{0}) because it is blacklisted."},
                    {"colorize_no_color_set", "[Colorize] You do not have a color currently set."},
                    {"colorize_reset_success", "[Colorize] You have successfully resetted your color!"},
                    {"colorize_no_permissions", "[Colorize] You do not have permissions for the color: #{0}."},
                };
            }
        }
    }
}
