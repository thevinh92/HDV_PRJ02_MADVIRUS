using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.MadVirus.Constants
{
    /// <summary>
    /// Thông số thiết đặt của game
    /// </summary>
    public class Configuration
    {
        public const string FacebookAppId = "431286293691374";

        // Virus Size
        public const float VIRUS_SPRITE_WIDTH = 84.0f;
        public const float VIRUS_SPRITE_HEIGHT = 74.0f;

        // Vitus Type
        public const int BLUE_VIRUS_ID = 2;
        public const int GREEN_VIRUS_ID = 4;
        public const int MAGENTA_VIRUS_ID = 1;
        public const int ORANGE_VIRUS_ID = 3;
        public const int RED_VIRUS_ID = 6;
        public const int YELLOW_VIRUS_ID = 5;

        public const int MAX_VIRUS_TYPES = 6;
    }
}
