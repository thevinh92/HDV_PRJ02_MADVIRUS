using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using WaveEngine.Framework.Services;


namespace HDV.MadVirus.Localization
{
    /// <summary>
    /// Các xâu được bản địa hóa
    /// </summary>
    public class LocalizedStrings
    {
        private static LocalizedStrings m_Instance;
        private static readonly Dictionary<Languages, Dictionary<string, string>> m_LocalizedStringDB;
        private static readonly Dictionary<Languages, string> m_LocalizedStringFiles =
            new Dictionary<Languages, string>
            {
                { Languages.Vietnamese, "Content/vi_strings.json" },
                { Languages.English, "Content/vi_strings.json" }
            };

        private static string ReadLocalizedStringFile(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {

            }
            return string.Empty;
        }

        static LocalizedStrings()
        {
            m_LocalizedStringDB = new Dictionary<Languages, Dictionary<string, string>>();

            foreach (var item in m_LocalizedStringFiles)
            {
                Languages language = item.Key;
                string fileName = item.Value;


                var localizedStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    ReadLocalizedStringFile(fileName));
                m_LocalizedStringDB.Add(language, localizedStrings);
            }
        }

        private Dictionary<string, string> m_LocalizedStringByLanguage;
        private Languages m_CurrentLanguage;
        private LocalizedStrings()
        {
            Language = Languages.Vietnamese;
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

        public Languages Language
        {
            set
            {
                this.m_CurrentLanguage = value;
                m_LocalizedStringByLanguage = m_LocalizedStringDB[value];
            }
            get
            {
                return m_CurrentLanguage;
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
