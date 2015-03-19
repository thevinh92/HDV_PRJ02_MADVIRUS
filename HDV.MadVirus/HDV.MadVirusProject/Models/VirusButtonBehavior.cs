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
    public delegate void ChooseColor(int color);
    class VirusButtonBehavior : Behavior
    {
        public event ChooseColor click;
        [RequiredComponent]
        public TouchGestures touchGest;
        public VirusButtonBehavior(int color)
        {
            colorID = color;

        }
        private int colorID;
        public int Color { get { return colorID; } }

        protected override void Initialize()
        {
            base.Initialize();
            touchGest.TouchTap += touchGestures_TouchTap;
        }
        void touchGestures_TouchTap(object sender, GestureEventArgs e)
        {
            if (click != null)
            {
                click(colorID);
            }
        }
        protected override void Update(TimeSpan gameTime)
        {

        }
    }
}
