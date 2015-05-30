using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.UI;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class GameMenuItem : BaseDecorator
    {
        private int m_Id;
        public GameMenuItem(int id, string title)
        {
            this.m_Id = id;

            this.entity = new Entity().
                AddComponent(new Transform2D
                {
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite("Content/game_menu_item_background.png")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha)).
                AddComponent(new RectangleCollider()).
                AddComponent(new TextControl("Content/fonts/tahoma.spr")
                {
                    Margin = Thickness.Zero,
                    Text = title,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new WaveEngine.Common.Graphics.Color("#00fce1")
                }).
                AddComponent(new TextControlRenderer()).
                AddComponent(new TouchGestures());


            //Touch
            var touchGestures = entity.FindComponent<TouchGestures>();
            touchGestures.TouchTap += OnTap;
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
            if (Clicked != null)
                Clicked(this, null);
        }

        public float X
        {
            set
            {
                var transform = entity.FindComponent<Transform2D>();
                transform.X = value;
            }
            get
            {
                var transform = entity.FindComponent<Transform2D>();
                return transform.X;
            }
        }

        public float Y
        {
            set
            {
                var transform = entity.FindComponent<Transform2D>();
                transform.Y = value;
            }
            get
            {
                var transform = entity.FindComponent<Transform2D>();
                return transform.Y;
            }
        }

        public void SetPosition(float x, float y)
        {
            var transform = entity.FindComponent<Transform2D>();
            transform.X = x;
            transform.Y = y;
        }

        public void SetPosition(WaveEngine.Common.Math.Vector2 position)
        {
            this.SetPosition(position.X, position.Y);
        }

        public int Id
        {
            get { return m_Id; }
        }

        public event EventHandler<EventArgs> Clicked;
    }
}
