using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace HDV.MadVirus.Entities.GUI
{
    internal struct FntCharacterRenderCommand
    {
        private Rectangle? m_SourceRectangle;
        private Texture2D m_Texture2D;
        private Vector2 m_Position;

        public FntCharacterRenderCommand(Texture2D texture, Rectangle sourceRect, Vector2 position)
        {
            this.m_SourceRectangle = new Rectangle?(sourceRect);
            this.m_Texture2D = texture;
            this.m_Position = position;
        }

        public Rectangle? SourceRect
        {
            get
            {
                return this.m_SourceRectangle;
            }
        }

        public Texture2D Texture2D
        {
            get
            {
                return this.m_Texture2D;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.m_Position;
            }
        }
    }

    public class FntTextControl : Component
    {
        static int instances;

        private string m_fntFontPath;
        private bool m_isGlobalAsset;
        private bool m_Disposed = false;
        private List<FntCharacterRenderCommand> m_CharacterRenderCommands;

        private string m_Text;

        [RequiredComponent]
        private Transform2D m_Transform;

        public FntFont FntFont
        {
            private set;
            get;
        }

        public FntTextControl(string fontPath, string text, bool isGlobalAsset) 
            : base("FntTextControl" + instances++)
        {
            this.m_fntFontPath = fontPath;
            this.m_isGlobalAsset = isGlobalAsset;
            this.m_Text = text;
            this.m_CharacterRenderCommands = new List<FntCharacterRenderCommand>();
            this.Color = Color.White;
        }

        public FntTextControl(string fontPath, string text)
            : this(fontPath, text, false)
        {
        }

        public FntTextControl(string fontPath)
            : this(fontPath, string.Empty)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            if (string.IsNullOrEmpty(this.m_fntFontPath))
            {
                throw new ArgumentException("Sprite Font Path is null");
            }

            if (m_isGlobalAsset)
            {
                this.FntFont = WaveServices.Assets.Global.LoadAsset<FntFont>(m_fntFontPath);
            }
            else
            {
                this.FntFont = base.Assets.LoadAsset<FntFont>(m_fntFontPath);
            }

            Layout();
        }

        public Color Color
        {
            set;
            get;
        }

        protected virtual void Layout()
        {
            if (FntFont == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(m_Text))
            {
                m_CharacterRenderCommands.Clear();
                return;
            }
            //Measure size
            Vector2 size = FntFont.MeasureString(m_Text);

            this.m_Transform.Rectangle.Width = size.X;
            this.m_Transform.Rectangle.Height = size.Y;

            //Layout
            m_CharacterRenderCommands.Clear();

            float cursorX, cursorY = 0;

            string[] lines = m_Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                //Bắt đầu một dòng mới
                string line = lines[lineIndex];

                bool isFirstLetterOfLine = true;
                char previousLetter = '?';

                cursorX = 0;

                for (int letterIndex = 0; letterIndex < line.Length; letterIndex++)
                {
                    char letter = line[letterIndex];

                    //Lấy thông tin của ký tự
                    Character letterInfo;
                    Texture2D letterTexture;
                    FntFont.GetLetterInfo(letter, out letterInfo, out letterTexture);

                    if (!isFirstLetterOfLine)
                    {
                        //Nếu không phải là ký tự đầu tiên của dòng
                        
                        //Dịch sang trái một đoạn XOffset
                        cursorX -= letterInfo.XOffset;

                        //Kịch sang trái thêm một đoạn khoảng cách Kerning với ký tự đứng trước
                        cursorX -= (float)FntFont.GetKerningAmount(previousLetter, letter);
                    }
                    else
                    {
                        isFirstLetterOfLine = false;
                    }

                    //Add Command
                    m_CharacterRenderCommands.Add(
                        new FntCharacterRenderCommand(letterTexture, 
                            letterInfo.TextureRegion, new Vector2(cursorX, cursorY + letterInfo.YOffset)));

                    previousLetter = letter;

                    //Dịch sang phải một đoạn XAdvance
                    cursorX += letterInfo.XAdvance;
                }

                cursorY += FntFont.LineHeight;
            }
        }

        public string Text
        {
            set
            {
                this.m_Text = value;
                Layout();
            }
            get
            {
                return this.m_Text;
            }
        }

        internal IEnumerable<FntCharacterRenderCommand> CharacterRenderCommands
        {
            get
            {
                return this.m_CharacterRenderCommands;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed && disposing)
            {
                if (!this.m_isGlobalAsset && !string.IsNullOrEmpty(m_fntFontPath))
                {
                    base.Assets.UnloadAsset(m_fntFontPath);
                }
            }

            this.m_Disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
