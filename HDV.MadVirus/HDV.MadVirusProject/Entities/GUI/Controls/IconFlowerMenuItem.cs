using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class IconFlowerMenuItem : FlowerMenuItem
    {
        public IconFlowerMenuItem(int id, string title, string iconPath)
            : base(id, title)
        {
            this.entity = new Entity().
                AddComponent(new Transform2D 
                {
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite(iconPath)).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha)).
                AddComponent(new RectangleCollider()).
                AddComponent(new TouchGestures());

            var touchGestures = entity.FindComponent<TouchGestures>();
            touchGestures.TouchTap += OnTap;
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
            if (Clicked != null)
                Clicked(this, null);
        }

        public event EventHandler<EventArgs> Clicked;
    }
}
