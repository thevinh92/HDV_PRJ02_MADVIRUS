using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Components.Cameras;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework.Services;


namespace HDV.MadVirus.Scenes
{
    public abstract class BaseScene : Scene
    {
        private float m_ViewportWidth;
        private float m_ViewportHeight;

        protected override void CreateScene()
        {
            //Tạo camera
            CreateCamera();

            //Tạo các đối tượng đồ họa
            CreateEntities();
        }

        protected virtual void CreateCamera()
        {
            //Tạo camera 2D
            var camera2D = new FixedCamera2D("Camera2D")
            {
                BackgroundColor = new Color("#e7e7e7"),
            };

            EntityManager.Add(camera2D);

            //Viewport Size
            var viewportManager = WaveServices.ViewportManager;
            this.m_ViewportWidth = viewportManager.VirtualWidth;
            this.m_ViewportHeight = viewportManager.VirtualHeight;
        }

        public float ViewportWidth
        {
            get { return m_ViewportWidth; }
        }

        public float ViewportHeight
        {
            get { return m_ViewportHeight; }
        }

        protected virtual void CreateEntities()
        {

        }
    }
}
