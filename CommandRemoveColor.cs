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
    public class CommandRemoveColor : IRocketCommand
    {   
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "removecolor";

        public string Help => "Removes your chat color.";

        public string Syntax => "/removecolor";

        public List<string> Aliases => new List<string> { "resetcolor" };

        public List<string> Permissions => new List<string>() { "colorize.remove" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer uPlayer = (UnturnedPlayer)caller;
            if (Colorize.instance.playerColors.TryGetValue(uPlayer.Id, out Color color))
            {
                Colorize.instance.playerColors.Remove(uPlayer.Id);
                UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_reset_success"), UnityEngine.Color.green);
            }
            else
            {
                UnturnedChat.Say(uPlayer, Colorize.instance.Translations.Instance.Translate("colorize_no_color_set"), UnityEngine.Color.red);
            }
        }
    }
}

