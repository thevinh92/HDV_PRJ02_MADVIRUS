using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDV.MadVirus.Localization
{
    /// <summary>
    /// Các xâu được bản địa hóa
    /// </summary>
    public class LocalizedStrings
    {
        private static LocalizedStrings m_Instance;

        private LocalizedStrings()
        {

        }

        public LocalizedStrings Current
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new LocalizedStrings();

                return m_Instance;
            }
        }

        /// <summary>
        /// Lấy nội dung xâu dựa theo Key
        /// </summary>
        /// <param name="key">Key của xâu</param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
