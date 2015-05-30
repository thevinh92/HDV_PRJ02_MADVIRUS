using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.Gestures;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics2D;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class CheckFlowerMenuItem : FlowerMenuItem
    {
        private Sprite m_CheckedSprite;
        private Sprite m_UncheckedSprite;
        private bool m_IsChecked = true;

        public CheckFlowerMenuItem(int id, string title, 
            string checkedImagePath, string uncheckedImagePath) : base(id, title)
        {
            this.m_CheckedSprite = new Sprite(checkedImagePath);
            this.m_UncheckedSprite = new Sprite(uncheckedImagePath);

            this.entity = new WaveEngine.Framework.Entity().
                AddComponent(new Transform2D
                {
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(m_CheckedSprite).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha)).
                AddComponent(new RectangleCollider()).
                AddComponent(new TouchGestures());

            var touchGestures = entity.FindComponent<TouchGestures>();
            touchGestures.TouchTap += OnTap;
        }

        public bool IsChecked
        {
            set
            {
                this.m_IsChecked = value;
                OnCheckedChanged();

                if (CheckedChanged != null)
                    CheckedChanged(this, null);
            }
            get
            {
                return m_IsChecked;
            }
        }

        private void OnCheckedChanged()
        {
            Sprite newSprite = null;

            if (m_IsChecked)
            {
                newSprite = m_CheckedSprite;
            }
            else
            {
                newSprite = m_UncheckedSprite;
            }

            entity.RemoveComponent<Sprite>();
            entity.AddComponent(newSprite);
            entity.RefreshDependencies();
        }

        private void OnTap(object sender, GestureEventArgs e)
        {
            IsChecked = !IsChecked;

            if (Clicked != null)
                Clicked(this, null);
        }

        public event EventHandler<EventArgs> Clicked;
        public event EventHandler<EventArgs> CheckedChanged;
    }
}
