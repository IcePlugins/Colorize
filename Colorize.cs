using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
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
            foreach (string c in Configuration.Instance.banned_colors)
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
                color = playerColor;
            }
        }

        public override TranslationList DefaultTranslations =>
            new TranslationList
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