using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class GameMenuItemClickedEventArgs : EventArgs
    {
        public GameMenuItem ClickedItem
        {
            set;
            get;
        }
    }

    public class GameMenu : BaseDecorator
    {
        private const float MenuItemHeight = 100;

        private Texture2D m_TransparentTexture;
        private Dictionary<int, GameMenuItem> m_MenuItems;
        public GameMenu()
        {
            this.m_MenuItems = new Dictionary<int, GameMenuItem>();

            //Create Temp Texture
            this.m_TransparentTexture = new Texture2D
            {
                Width = 1,
                Height = 1,
                Levels = 1,
                Data = new byte[1][][] { new byte[1][] { new byte[] { 0, 255, 255, 255 } } }
            };

            WaveServices.GraphicsDevice.Textures.UploadTexture(m_TransparentTexture);

            //Create entity
            this.entity = new Entity().
                AddComponent(new Transform2D
                {
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite(m_TransparentTexture)).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
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
            SetPosition(position.X, position.Y);
        }

        public GameMenuItem AddItem(int itemId, string menuTitle)
        {
            if (m_MenuItems.ContainsKey(itemId))
                throw new ArgumentException("Contained this item");

            if (string.IsNullOrEmpty(menuTitle))
                throw new ArgumentNullException();

            GameMenuItem menuItem = new GameMenuItem(itemId, menuTitle);
            menuItem.Clicked += OnItemClicked;
            m_MenuItems.Add(itemId, menuItem);
            
            entity.AddChild(menuItem.Entity);

            Arrage();

            return menuItem;
        }

        public void RemoveItem(int itemId)
        {
            if (!m_MenuItems.ContainsKey(itemId))
                return;

            GameMenuItem menuItem = m_MenuItems[itemId];
            m_MenuItems.Remove(itemId);
            entity.RemoveChild(menuItem.Entity.Name);

            Arrage();
        }

        private void Arrage()
        {
            float currentY = 0;

            var menuItems = m_MenuItems.Values.Reverse();
            foreach (var menuItem in menuItems)
            {
                menuItem.Y = currentY;
                currentY -= MenuItemHeight;
            }
        }

        private void OnItemClicked(object sender, EventArgs e)
        {
            GameMenuItem menuItem = sender as GameMenuItem;

            if (MenuItemClicked != null)
                MenuItemClicked(this, new GameMenuItemClickedEventArgs { ClickedItem = menuItem });
        }

        public override void Dispose()
        {
            base.Dispose();

            WaveServices.GraphicsDevice.Textures.DestroyTexture(m_TransparentTexture);
        }

        public event EventHandler<GameMenuItemClickedEventArgs> MenuItemClicked;
    }
}
