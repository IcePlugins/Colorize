using Rocket.API;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ExtraConcentratedJuice.Colorize
{
    public class ColorizeConfig : IRocketPluginConfiguration
    {
        [XmlArrayItem(ElementName = "color")]
        public List<string> banned_colors;
        public Boolean enable_bypass_permission;

        public void LoadDefaults()
        {
            banned_colors = new List<string>() { "#FFFFFF", "#FFF000" };
            enable_bypass_permission = true;
        }
    }
}
