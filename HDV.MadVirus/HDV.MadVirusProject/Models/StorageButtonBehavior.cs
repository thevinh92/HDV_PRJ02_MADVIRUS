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
    public delegate void Save();
    class StorageButtonBehavior : Behavior
    {
        public event Save clickSave;
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
            if(clickSave != null)
            {
                clickSave();
            }
        }
    }
}
