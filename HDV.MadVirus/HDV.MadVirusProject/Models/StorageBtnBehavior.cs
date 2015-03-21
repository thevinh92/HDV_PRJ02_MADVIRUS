using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Framework;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Input;
using WaveEngine.Components.Gestures;

namespace HDV.MadVirus.Models
{
    public delegate void Load();
    class StorageBtnBehavior : Behavior
    {
        public event Load clickLoad;
        [RequiredComponent]
        public TouchGestures touchGest;

        protected override void Initialize()
        {
            base.Initialize();
            touchGest.TouchTap += touchGest_TouchTap;
        }

        protected override void Update(TimeSpan gameTime)
        {

        }

        void touchGest_TouchTap(object sender, GestureEventArgs e)
        {
            if (clickLoad != null)
            {
                clickLoad();
            }
        }
    }
}
