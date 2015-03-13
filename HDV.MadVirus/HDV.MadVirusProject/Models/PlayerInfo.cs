using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.MadVirus.Models
{
    /// <summary>
    /// Thông tin người chơi
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// ID trên Facebook của người chơi
        /// </summary>
        public string ID
        {
            set;
            get;
        }

        /// <summary>
        /// Đường dẫn tới Avatar của người chơi
        /// </summary>
        public string AvatarUrl
        {
            set;
            get;
        }
    }
}
