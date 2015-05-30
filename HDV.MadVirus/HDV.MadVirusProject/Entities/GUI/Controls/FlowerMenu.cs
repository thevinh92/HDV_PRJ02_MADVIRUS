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
using WaveEngine.Framework.Services;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class FlowerMenuItemClickEventArgs : EventArgs
    {
        public FlowerMenuItem ClickedItem
        {
            set;
            get;
        }
    }

    public class FlowerMenuItemCheckedChangedEventArgs : EventArgs
    {
        public CheckFlowerMenuItem CheckedMenuItem
        {
            set;
            get;
        }
    }

    public class FlowerMenuClickEventArgs : EventArgs
    {
        public bool IsCancelShowItems
        {
            set;
            get;
        }
    }

    public enum FlowMenuDirections
    {
        UpRight = 0,
        UppLeft = 1,
        DownLeft = 2,
    }

    public class FlowerMenu : BaseDecorator
    {
        private class ItemContainer : BaseDecorator
        {
            private const float Radius = 80;

            private Dictionary<int, FlowerMenuItem> m_Items;
            private Texture2D m_TempTexture;
            private FlowMenuDirections m_Direction;
            public ItemContainer()
            {
                this.m_TempTexture = new Texture2D 
                {
                    Width = 1,
                    Height = 1,
                    Levels = 1,
                    Format = WaveEngine.Common.Graphics.PixelFormat.R8G8B8A8,
                    Data = new byte[1][][] { new byte[1][] { new byte[] { 0, 255, 255, 255 } } }
                };

                WaveServices.GraphicsDevice.Textures.UploadTexture(m_TempTexture);

                this.entity = new Entity().
                    AddComponent(new Transform2D 
                    { 
                        X = 200,
                        Y = -200,
                        Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                    }).
                    AddComponent(new Sprite(m_TempTexture)).
                    AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

                this.m_Items = new Dictionary<int, FlowerMenuItem>();
            }

            public void Add(FlowerMenuItem item)
            {
                m_Items.Add(item.Id, item);
                entity.AddChild(item.Entity);
                OnArrage();
            }

            public void Remove(FlowerMenuItem item)
            {
                m_Items.Remove(item.Id);
                entity.RemoveChild(item.Entity.Name);
                OnArrage();
            }

            private void OnArrage()
            {
                var items = m_Items.Values;
                var transform = entity.FindComponent<Transform2D>();

                if (m_Direction == FlowMenuDirections.UpRight)
                {
                    double currentAngle =  Math.PI / 6;

                    foreach (var item in items)
                    {
                        float x = -(float)(Math.Cos(currentAngle) * Radius);
                        float y = -(float)(Math.Sin(currentAngle) * Radius);

                        var itemTransform = item.Entity.FindComponent<Transform2D>();
                        itemTransform.X = x;
                        itemTransform.Y = y;

                        currentAngle += Math.PI / 3;
                    }

                    transform.X = 70;
                    transform.Y = -70;
                } 
                else if (m_Direction == FlowMenuDirections.UppLeft)
                {
                    double currentAngle = Math.PI / 6;

                    foreach (var item in items)
                    {
                        float x = (float)(Math.Cos(currentAngle) * Radius);
                        float y = -(float)(Math.Sin(currentAngle) * Radius);

                        var itemTransform = item.Entity.FindComponent<Transform2D>();
                        itemTransform.X = x;
                        itemTransform.Y = y;

                        currentAngle += Math.PI / 3;
                    }

                    transform.X = -70;
                    transform.Y = -70;
                }
                else
                {

                }
            }

            public FlowerMenuItem FindItemById(int id)
            {
                if (m_Items.ContainsKey(id))
                    return m_Items[id];

                return null;
            }

            public FlowMenuDirections Direction
            {
                set
                {
                    this.m_Direction = value;
                    OnArrage();
                }
                get
                {
                    return m_Direction;
                }
            }

            public override void Dispose()
            {
                base.Dispose();

                WaveServices.GraphicsDevice.Textures.DestroyTexture(m_TempTexture);
            }
        }

        private ItemContainer m_ItemContainer;

        public FlowerMenu(string iconImagePath)
        {
            this.entity = new Entity().
                AddComponent(new Transform2D 
                {
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite(iconImagePath)).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha)).
                AddComponent(new RectangleCollider()).
                AddComponent(new TouchGestures());

            var touchGesture = entity.FindComponent<TouchGestures>();
            touchGesture.TouchTap += OnTouchTap;

            //Items Container
            this.m_ItemContainer = new ItemContainer();
            entity.AddChild(m_ItemContainer.Entity);

            this.entity.EntityInitialized += OnInitialized;
            
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            m_ItemContainer.IsVisible = false;
            m_ItemContainer.Entity.RefreshDependencies();
        }

        private void OnTouchTap(object sender, GestureEventArgs e)
        {
            FlowerMenuClickEventArgs menuClickEventArgs = new FlowerMenuClickEventArgs();
            if (Clicked != null)
                Clicked(this, menuClickEventArgs);

            if (!menuClickEventArgs.IsCancelShowItems)
            {
                if (!IsItemsShow)
                    ShowItems();
                else
                    HideItems();
            }
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

        public void AddIconItem(int id, string title, string iconPath)
        {
            IconFlowerMenuItem iconItem = new IconFlowerMenuItem(id, title, iconPath);
            AddItem(iconItem);
        }

        public void AddCheckItem(int id, string title, 
            string checkedImagePath, string uncheckedImagePath)
        {
            CheckFlowerMenuItem checkMenuItem = new CheckFlowerMenuItem(id, title, 
                checkedImagePath, uncheckedImagePath);
            checkMenuItem.CheckedChanged += OnMenuItemCheckedChanged;
            AddItem(checkMenuItem);
        }
        
        public void AddItem(FlowerMenuItem item)
        {
            int id = item.Id;
            if (m_ItemContainer.FindItemById(id) != null)
                return;

            m_ItemContainer.Add(item);
            if (item is IconFlowerMenuItem)
            {
                var iconItem = item as IconFlowerMenuItem;
                iconItem.Clicked += OnItemClick;
            } 
            else if (item is CheckFlowerMenuItem)
            {
                var iconItem = item as CheckFlowerMenuItem;
                iconItem.Clicked += OnItemClick;
            }
        }

        public void RemoveItem(int id)
        {
            var item = m_ItemContainer.FindItemById(id);
            if (item == null)
                return;

            m_ItemContainer.Remove(item);
        }

        public void ShowItems()
        {
            m_ItemContainer.IsVisible = true;
        }

        public void HideItems()
        {
            m_ItemContainer.IsVisible = false;
        }

        public bool IsItemsShow
        {
            get
            {
                return m_ItemContainer.IsVisible;
            }
        }

        public FlowMenuDirections Direction
        {
            set
            {
                m_ItemContainer.Direction = value;
            }
            get
            {
                return m_ItemContainer.Direction;
            }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            HideItems();

            if (IconMenuItemClicked != null)
                IconMenuItemClicked(this, 
                    new FlowerMenuItemClickEventArgs 
                    { 
                        ClickedItem = sender as FlowerMenuItem 
                    });
        }

        private void OnMenuItemCheckedChanged(object sender, EventArgs e)
        {
            CheckFlowerMenuItem checkMenuItem = sender as CheckFlowerMenuItem;
            if (MenuItemCheckedChanged != null)
                MenuItemCheckedChanged(this, new FlowerMenuItemCheckedChangedEventArgs 
                {
                    CheckedMenuItem = checkMenuItem
                });
        }

        /// <summary>
        /// Sự kiện khi Icon của menu được ấn
        /// </summary>
        public event EventHandler<FlowerMenuClickEventArgs> Clicked;

        /// <summary>
        /// Sự kiện khi Icon Item được click
        /// </summary>
        public event EventHandler<FlowerMenuItemClickEventArgs> IconMenuItemClicked;

        public event EventHandler<FlowerMenuItemCheckedChangedEventArgs> MenuItemCheckedChanged;
    }
}
