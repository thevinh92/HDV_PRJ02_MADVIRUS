using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace HDV.MadVirus.Entities.GUI
{
    public struct Int32Padding
    {
        private int m_Up;
        private int m_Right;
        private int m_Down;
        private int m_Left;

        public Int32Padding(int up, int right, int down, int left)
        {
            this.m_Up = up;
            this.m_Right = right;
            this.m_Down = down;
            this.m_Left = left;
        }

        public int Up
        {
            set
            {
                this.m_Up = value;
            }
            get
            {
                return this.m_Up;
            }
        }

        public int Right
        {
            set
            {
                this.m_Right = value;
            }
            get
            {
                return this.m_Right;
            }
        }

        public int Down
        {
            set
            {
                this.m_Down = value;
            }
            get
            {
                return this.m_Down;
            }
        }

        public int Left
        {
            set
            {
                this.m_Left = value;
            }
            get
            {
                return this.m_Left;
            }
        }

        public static Int32Padding NoPadding
        {
            get
            {
                return default(Int32Padding);
            }
        }
    }

    internal struct Page
    {
        private int m_Id;
        private string m_FileName;
        public Page(int id, string fileName)
        {
            this.m_Id = id;
            this.m_FileName = fileName;
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public string FileName
        {
            set
            {
                this.m_FileName = value;
            }
            get
            {
                return this.m_FileName;
            }
        }

        public override string ToString()
        {
            return string.Format("Page: id = {0},  file = {1}", m_Id, m_FileName);
        }
    }

    internal struct Character
    {
        private int m_Id;
        private Rectangle m_TextureRegion;
        private char m_Letter;
        private int m_PageId;
        private int m_XOffset;
        private int m_YOffset;
        private int m_XAdvance;

        public Character(int id, Rectangle textureRegion, int pageId, char letter,
            int xoffset, int yoffset, int xadvance)
        {
            this.m_Id = id;
            this.m_TextureRegion = textureRegion;
            this.m_PageId = pageId;
            this.m_Letter = letter;
            this.m_XOffset = xoffset;
            this.m_YOffset = yoffset;
            this.m_XAdvance = xadvance;
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public Rectangle TextureRegion
        {
            get
            {
                return m_TextureRegion;
            }
        }

        public int PageId
        {
            get
            {
                return m_PageId;
            }
        }

        public char Letter
        {
            get
            {
                return m_Letter;
            }
        }

        public int XOffset
        {
            get
            {
                return this.m_XOffset;
            }
        }

        public int YOffset
        {
            get
            {
                return this.m_YOffset;
            }
        }

        public int XAdvance
        {
            get
            {
                return m_XAdvance;
            }
        }

        public override string ToString()
        {
            return string.Format("Character \'{0}\'", m_Letter);
        }
    }

    internal struct Kerning
    {
        private char m_FirstChar;
        private char m_SecondChar;
        private int m_Amount;

        public Kerning(char firstChar, char secondChar, int amount)
        {
            this.m_FirstChar = firstChar;
            this.m_SecondChar = secondChar;
            this.m_Amount = amount;
        }

        public char FirstChar
        {
            get
            {
                return this.m_FirstChar;
            }
        }

        public char SecondChar
        {
            get
            {
                return this.m_SecondChar;
            }
        }

        public int Amount
        {
            get
            {
                return this.m_Amount;
            }
        }

        public override string ToString()
        {
            return string.Format("Kerning: first=\'{0}\' second=\'{1}\' amount={2}",
                m_FirstChar, m_SecondChar, m_Amount);
        }
    }

    #region Tag Parameter Helper
    public static class ParametersDictionaryExtension
    {
        public static string GetFieldAsString(this Dictionary<string, string> _this, string fieldName, string defaultValue = null)
        {
            string stringValue;
            if (!_this.TryGetValue(fieldName, out stringValue))
                return defaultValue;

            stringValue = stringValue.Replace("\"", string.Empty);
            return stringValue;
        }

        public static int GetFieldAsInt(this Dictionary<string, string> _this, string fieldName, int defaultValue = 0)
        {
            string stringValue;
            if (!_this.TryGetValue(fieldName, out stringValue))
                return defaultValue;

            int value;
            if (!int.TryParse(stringValue, out value))
                return defaultValue;

            return value;
        }

        public static bool GetFieldAsBool(this Dictionary<string, string> _this, string fieldName, bool defaultValue = false)
        {
            string stringValue;
            if (!_this.TryGetValue(fieldName, out stringValue))
                return defaultValue;

            int value;
            if (!int.TryParse(stringValue, out value))
                return defaultValue;

            return value > 0;
        }

        public static Int32Padding GetFieldAsPadding(this Dictionary<string, string> _this, string fieldName)
        {
            string stringValue;
            if (!_this.TryGetValue(fieldName, out stringValue))
                return Int32Padding.NoPadding;

            string[] fieldSplitted = stringValue.Split(',');
            if (fieldSplitted.Length < 4)
                return Int32Padding.NoPadding;

            return new Int32Padding(int.Parse(fieldSplitted[0]), int.Parse(fieldSplitted[1]),
                int.Parse(fieldSplitted[2]), int.Parse(fieldSplitted[3]));
        }

        public static Point GetFieldAsPoint(this Dictionary<string, string> _this, string fieldName)
        {
            string stringValue;
            if (!_this.TryGetValue(fieldName, out stringValue))
                return new Point(0, 0);

            string[] fieldSplitted = stringValue.Split(',');
            if (fieldSplitted.Length < 2)
                return new Point(0, 0);

            return new Point(int.Parse(fieldSplitted[0]), int.Parse(fieldSplitted[1]));
        }
    }
    #endregion

    public sealed class FntFont : ILoadable<GraphicsDevice>, ILoadable
    {
        private GraphicsDevice m_GraphicDevice;
        private Dictionary<int, Page> m_Pages;
        private Dictionary<string, Texture2D> m_Textures;
        private Dictionary<char, Character> m_Characters;
        private Dictionary<Tuple<char, char>, Kerning> m_Kernings;

        #region Info
        public string FontName
        {
            private set;
            get;
        }

        public int FontSize
        {
            private set;
            get;
        }

        public bool IsBold
        {
            private set;
            get;
        }

        public bool IsItalic
        {
            private set;
            get;
        }

        public string Charset
        {
            private set;
            get;
        }

        public bool IsUnicode
        {
            private set;
            get;
        }

        public int StretchHeightPercentage
        {
            private set;
            get;
        }

        public bool IsSmooth
        {
            private set;
            get;
        }

        public bool IsSuperSampling
        {
            private set;
            get;
        }

        public Int32Padding Padding
        {
            private set;
            get;
        }

        public Point Space
        {
            private set;
            get;
        }

        public int Outline
        {
            private set;
            get;
        }
        #endregion

        #region Common
        public int LineHeight
        {
            private set;
            get;
        }

        public int BaseLine
        {
            private set;
            get;
        }

        public int ScaleWidth
        {
            private set;
            get;
        }

        public int ScaleHeight
        {
            private set;
            get;
        }
        #endregion

        public FntFont()
        {
            this.m_Pages = new Dictionary<int, Page>();
            this.m_Textures = new Dictionary<string, Texture2D>();
            this.m_Characters = new Dictionary<char, Character>();
            this.m_Kernings = new Dictionary<Tuple<char, char>, Kerning>();
        }

        public void Load(GraphicsDevice graphicsDevice, System.IO.Stream stream)
        {
            this.m_GraphicDevice = graphicsDevice;

            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    //File Name
                    string fileName = reader.ReadString();

                    //File Length
                    int fileLength = reader.ReadInt32();

                    //Buffer
                    byte[] buffer = reader.ReadBytes(fileLength);

                    MemoryStream fileStream = new MemoryStream(buffer);

                    string fileExtension = System.IO.Path.GetExtension(fileName).ToUpper();
                    if (fileExtension.Equals(".FNT", StringComparison.CurrentCultureIgnoreCase))
                    {
                        
                        LoadFntFile(fileStream);
                    } 
                    else if (fileExtension.Equals(".PNG", StringComparison.CurrentCultureIgnoreCase) ||
                        fileExtension.Equals(".JPG", StringComparison.CurrentCultureIgnoreCase) ||
                        fileExtension.Equals(".JPEG", StringComparison.CurrentCultureIgnoreCase))
                    {
                        LoadTextureFile(fileName, fileStream);
                    }

                    fileStream.Close();
                }
            }
        }

        #region Fnt File Process
        private static readonly string fntTagRegex = @"^\w+(\s|$)|\w+\=(\d+(\s|$)|\d+\,\d+(\s|$)|""[^""]*""|\d+\,\d+\,\d+\,\d+)";

        private void LoadFntFile(Stream stream)
        {
            Regex regex = new Regex(fntTagRegex);

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    //Đọc một dòng trong file
                    string line = reader.ReadLine();

                    string tagName = string.Empty;
                    Dictionary<string, string> parameters = new Dictionary<string, string>();

                    MatchCollection matches = regex.Matches(line);
                    foreach (Match match in matches)
                    {
                        if (!match.Success)
                            continue;

                        string matchValue = match.Value;
                        if (!matchValue.Contains("="))
                        {
                            tagName = matchValue.Trim();
                        }
                        else
                        {
                            string[] keyValueSplitted = matchValue.Split('=');
                            parameters.Add(keyValueSplitted[0].Trim(), keyValueSplitted[1].Trim());
                        }
                    }

                    if (string.IsNullOrEmpty(tagName))
                        continue;

                    ProcessTag(tagName, parameters);
                }
            }
        }

        private void ProcessTag(string tagName, Dictionary<string, string> parameters)
        {
            if (tagName.Equals("info", StringComparison.CurrentCultureIgnoreCase))
            {
                ProcessInfoTag(parameters);
            }
            else if (tagName.Equals("common", StringComparison.CurrentCultureIgnoreCase))
            {
                ProcessCommonTag(parameters);
            }
            else if (tagName.Equals("page", StringComparison.CurrentCultureIgnoreCase))
            {
                ProcessPageTag(parameters);
            } 
            else if (tagName.Equals("char", StringComparison.CurrentCultureIgnoreCase))
            {
                ProcessCharTag(parameters);
            } 
            else if (tagName.Equals("kerning", StringComparison.CurrentCultureIgnoreCase))
            {
                ProcessKerningTag(parameters);
            }
        }

        private void ProcessInfoTag(Dictionary<string, string> parameters)
        {
            this.FontName = parameters.GetFieldAsString("font");
            this.FontSize = parameters.GetFieldAsInt("size");
            this.IsBold = parameters.GetFieldAsBool("bold");
            this.IsItalic = parameters.GetFieldAsBool("italic");
            this.Charset = parameters.GetFieldAsString("charset");
            this.IsUnicode = parameters.GetFieldAsBool("unicode");
            this.StretchHeightPercentage = parameters.GetFieldAsInt("stretchH");
            this.IsSmooth = parameters.GetFieldAsBool("smooth");
            this.IsSuperSampling = parameters.GetFieldAsBool("aa");
            this.Padding = parameters.GetFieldAsPadding("padding");
            this.Space = parameters.GetFieldAsPoint("spacing");
            this.Outline = parameters.GetFieldAsInt("outline");
        }

        private void ProcessCommonTag(Dictionary<string, string> parameters)
        {
            this.LineHeight = parameters.GetFieldAsInt("lineHeight");
            this.BaseLine = parameters.GetFieldAsInt("base");
            this.ScaleWidth = parameters.GetFieldAsInt("scaleW");
            this.ScaleHeight = parameters.GetFieldAsInt("scaleH");
        }

        private void ProcessPageTag(Dictionary<string, string> parameters)
        {
            int pageId = parameters.GetFieldAsInt("id");
            string textureFile = parameters.GetFieldAsString("file");
            
            var page = new Page(pageId, textureFile);
            m_Pages.Add(pageId, page);
        }

        private void ProcessCharTag(Dictionary<string, string> parameters)
        {
            int characterId = parameters.GetFieldAsInt("id");
            int x = parameters.GetFieldAsInt("x");
            int y = parameters.GetFieldAsInt("y");
            int width = parameters.GetFieldAsInt("width");
            int height = parameters.GetFieldAsInt("height");
            int page = parameters.GetFieldAsInt("page");
            int xoffset = parameters.GetFieldAsInt("xoffset");
            int yoffset = parameters.GetFieldAsInt("yoffset");
            int xadvance = parameters.GetFieldAsInt("xadvance");

            char letter;
            string letterString = parameters.GetFieldAsString("letter");
            if (string.IsNullOrEmpty(letterString))
                letter = (char)characterId;
            else
                letter = char.Parse(letterString);

            Character character = new Character(characterId, new Rectangle(x, y, width, height), 
                page, letter, xoffset, yoffset, xadvance);
            m_Characters.Add(letter, character);
        }

        private void ProcessKerningTag(Dictionary<string, string> parameters)
        {
            char firstChar = (char)parameters.GetFieldAsInt("first");
            char secondChar = (char)parameters.GetFieldAsInt("second");
            int amount = parameters.GetFieldAsInt("amount");

            Kerning kerning = new Kerning(firstChar, secondChar, amount);
            m_Kernings.Add(Tuple.Create<char, char>(firstChar, secondChar), kerning);
        }
        #endregion

        private void LoadTextureFile(string fileName, Stream stream)
        {
            Texture2D texture = Texture2D.FromFile(m_GraphicDevice, stream);
            m_Textures.Add(fileName, texture);
        }

        public string AssetPath
        {
            set;
            get;
        }

        public Version ReaderVersion
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public Vector2 MeasureString(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Vector2.Zero;

            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            float blockHeight = lines.Length * LineHeight;
            float blockWidth = 0;
            
            foreach (var line in lines)
            {
                int measuredWidth = MeasureLineWidth(line);
                
                if (measuredWidth > blockWidth)
                    blockWidth = measuredWidth;
            }

            return new Vector2(blockWidth, blockHeight);
        }

        private int MeasureLineWidth(string line)
        {
            int width = 0;

            char previousLetter = '?';
            for (int letterIndex = 0; letterIndex < line.Length; letterIndex++)
            {
                char letter = line[letterIndex];

                Character letterInfo = default(Character);

                if (m_Characters.ContainsKey(letter))
                    letterInfo = m_Characters[letter];
                else
                    letterInfo = m_Characters['?'];

                if (letterIndex != 0)
                {
                    if (letterIndex != line.Length - 1)
                        width += letterInfo.XAdvance;
                    else
                        width += (letterInfo.TextureRegion.Width - letterInfo.XOffset);

                    width -= GetKerningAmount(previousLetter, letter);
                }
                else
                {
                    width += letterInfo.XOffset;
                    width += letterInfo.XAdvance;
                }

                previousLetter = letter;
            }

            return width;
        }

        public void Unload()
        {
            foreach (var texture in m_Textures.Values)
            {
                texture.Unload();
            }
        }

        internal void GetLetterInfo(char letter, out Character character, out Texture2D texture)
        {
            character = default(Character);
            texture = default(Texture2D);

            if (!m_Characters.ContainsKey(letter))
                letter = '?';

            character = m_Characters[letter];
            texture = m_Textures[m_Pages[character.PageId].FileName];
        }

        internal int GetKerningAmount(char firstLetter, char secondLetter)
        {
            var kerningKey = Tuple.Create<char, char>(firstLetter, secondLetter);
            if (!m_Kernings.ContainsKey(kerningKey))
                return 0;

            return m_Kernings[kerningKey].Amount;
        }
    }
}
