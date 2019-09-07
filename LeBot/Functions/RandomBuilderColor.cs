using System;
using System.Collections.Generic;
using Discord;
using System.Text;

namespace LeBot.Functions
{
    class RandomBuilderColor
    {
        public Discord.Color MessageBuilderColor()
        {
            Random builderColor = new Random();
            int randomColor = builderColor.Next(0, 19);
            var colors = new[] {Color.Blue, Color.DarkBlue, Color.DarkGreen, Color.DarkGrey, Color.DarkMagenta, Color.DarkOrange, Color.DarkPurple, Color.DarkRed, Color.DarkTeal, Color.Gold, Color.Green, Color.LighterGrey, Color.LightGrey, Color.LightOrange, Color.Magenta, Color.Orange, Color.Purple, Color.Red, Color.Teal};
            return colors[randomColor];
        }
    }
}
