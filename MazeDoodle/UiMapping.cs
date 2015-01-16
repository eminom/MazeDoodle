using System;
using System.Collections.Generic;
using System.Text;

namespace MazeDoodle
{
    internal class UiMapping
    {
        public static int MapVeloStringToInt(String velo)
        {
            int res = 0;
            switch (velo.ToLower())
            {
                default:
                case "normal":
                    res = 0;
                    break;
                case "fast":
                    res = 1;
                    break;
            }
            return res;
        }

        public static int MapSkinStringToInt(String skin)
        {
            int res = 0;
            switch(skin.ToLower())
            {
                default:
                case "ranger":
                    res = 0;
                    break;
                case "android":
                    res = 1;
                    break;
                case "gundam":
                    res = 2;
                    break;
            }
            return res;
        }

    }
}
