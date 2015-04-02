using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;

using HDV.MadVirus.Entities.GUI.Controls;
using WaveEngine.Components.UI;
using WaveEngine.Components.Animation;

namespace HDV.MadVirus.Scenes
{
    /// <summary>
    /// Màn hình Splash
    /// </summary>
    public class SplashScene : BaseScene
    {
        #region Layer Order
        const float BackgroundLayerOrder = 1.0f;
        const float LogoLayerOrder = 0.9f;
        const float ProgressBarOrder = 0.8f;
        #endregion

        const string LoadingAnimation = "loadingAnimation";

        const double SplashDuration = 1000; //2000ms
        const int SplashFPS = 4;

        private Entity m_BackgroundEntity;
        private Entity m_SplashLogo;
        private HDV.MadVirus.Entities.GUI.Controls.ProgressBar m_ProgressBar;
        private Entity m_LoadingLabel;

        protected override void CreateEntities()
        {
            base.CreateEntities();

            //Background
            this.m_BackgroundEntity = new Entity("background").
                AddComponent(new Transform2D {
                    X = ViewportWidth / 2,
                    Y = ViewportHeight / 2,
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f),
                    DrawOrder = BackgroundLayerOrder
                }).
                AddComponent(new Sprite("Content/background.wpk")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(m_BackgroundEntity);

            //Logo
            this.m_SplashLogo = new Entity("splash_logo").
                AddComponent(new Transform2D {
                    X = ViewportWidth / 2,
                    Y = ViewportHeight / 3,
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f),
                    DrawOrder = LogoLayerOrder,
                }).
                AddComponent(new Sprite("Content/splash_logo.wpk")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            EntityManager.Add(m_SplashLogo);

            //Progress Bar
            this.m_ProgressBar = new HDV.MadVirus.Entities.GUI.Controls.ProgressBar();
            var progressBarTransform = m_ProgressBar.Entity.FindComponent<Transform2D>();
            progressBarTransform.X = ViewportWidth / 2;
            progressBarTransform.Y = ViewportHeight * 3 / 4;
            progressBarTransform.DrawOrder = ProgressBarOrder;
            m_ProgressBar.Percentage = 0.5f;

            EntityManager.Add(m_ProgressBar.Entity);

            //Label 
            this.m_LoadingLabel = new Entity("loadingLabel").
                AddComponent(new Transform2D 
                { 
                    X = ViewportWidth / 2,
                    Y = ViewportHeight * 0.65f,
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite("Content/progress_bar/loading_atlas.wpk")).
                AddComponent(Animation2D.Create<TexturePackerGenericXml>("Content/progress_bar/loading_atlas.xml").
                    Add(LoadingAnimation, new SpriteSheetAnimationSequence
                    {
                        First = 0,
                        Length = 4,
                        FramesPerSecond = SplashFPS,
                    })).
                AddComponent(new AnimatedSpriteRenderer());

            EntityManager.Add(m_LoadingLabel);
        }

        protected override void Start()
        {
            base.Start();

            //Start Loading
            m_ProgressBar.StartAnimation(0f, 1.0f, SplashDuration, OnLoadingCompleted);

            var animation = m_LoadingLabel.FindComponent<Animation2D>();
            animation.CurrentAnimation = LoadingAnimation;
            animation.Play(true);
        }

        private void OnLoadingCompleted()
        {
            WaveServices.ScreenContextManager.Push(
                new ScreenContext("homeScene", new HomeScene()));
        }
    }
}
