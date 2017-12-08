using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ExtraConcentratedJuice.Colorize
{
    public class CommandColorize : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "colorize";

        public string Help => "Sets your chat color.";

        public string Syntax => "/colorize <color>";

        public List<string> Aliases => new List<string> { "setcolor" };

        public List<string> Permissions => new List<string>() { "colorize" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;
            if (args.Length != 1)
            {
                UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_invalid_args"), UnityEngine.Color.red);
                return;
            }
            if (uPlayer.HasPermission("colorize." + args[0]) || uPlayer.HasPermission("colorize.#" + args[0]) || uPlayer.HasPermission("colorize.*"))
            {

                if (args[0].StartsWith("#"))
                {
                    Color color = (Color)UnturnedChat.GetColorFromHex(args[0]);
                    if (color == null)
                    {
                        UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_invalid_color"), UnityEngine.Color.red);
                        return;
                    }
                    if (IsBlacklistedColor(color, uPlayer))
                    {
                        if (!uPlayer.HasPermission("colorizer.bypass") && !Colorize.instance.Configuration.Instance.enable_bypass_permission)
                        {
                            UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_blacklisted", ColorUtility.ToHtmlStringRGB(color)), UnityEngine.Color.red);
                            return;
                        }
                    }
                    Colorize.instance.playerColors[uPlayer.Id] = color;
                    UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_success", ColorUtility.ToHtmlStringRGB(color)), UnityEngine.Color.green);

                }
                else
                {
                    Color color = (Color)UnturnedChat.GetColorFromName(args[0], UnityEngine.Color.clear);
                    if (IsBlacklistedColor(color, uPlayer))
                    {
                        if(!uPlayer.HasPermission("colorizer.bypass") && !Colorize.instance.Configuration.Instance.enable_bypass_permission)
                        {
                            UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_blacklisted", ColorUtility.ToHtmlStringRGB(color)), UnityEngine.Color.red);
                            return;
                        }

                    }
                    Colorize.instance.playerColors[uPlayer.Id] = color;
                    UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_success", ColorUtility.ToHtmlStringRGB(color)), UnityEngine.Color.green);


                }
            }
            else
            {
                UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_no_permissions", args[0]), UnityEngine.Color.red);
            }
        }

        public static bool IsBlacklistedColor(Color color, UnturnedPlayer player)
        {
            foreach (string c in Colorize.instance.Configuration.Instance.banned_colors)
            {
                if (ColorUtility.ToHtmlStringRGB(color) == c || "#" + ColorUtility.ToHtmlStringRGB(color) == c)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

