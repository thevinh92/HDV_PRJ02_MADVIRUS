#region Using Statements
using System;
using WaveEngine.Common;
using WaveEngine.Common.Graphics;
using WaveEngine.Framework;
using WaveEngine.Framework.Services;

using HDV.MadVirus.Scenes;
#endregion

namespace HDV.MadVirus
{
    public class Game : WaveEngine.Framework.Game
    {
        public override void Initialize(IApplication application)
        {
            base.Initialize(application);

            // ViewportManager is used to automatically adapt resolution to fit screen size
            ViewportManager vm = WaveServices.ViewportManager;
            //vm.Activate(1280, 720, ViewportManager.StretchMode.Uniform);
            
            ScreenContext screenContext = new ScreenContext(new SplashScene());
            vm.Activate(1280, 768, ViewportManager.StretchMode.Fill);
           // ScreenContext screenContext = new ScreenContext(new GamePlayScene());
           // ScreenContext screenContext = new ScreenContext(new GamePlayScene(true));
            WaveServices.ScreenContextManager.To(screenContext);

        }
    }

}
