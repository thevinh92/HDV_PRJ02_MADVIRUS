using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Framework;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class FlowerMenuItem : BaseDecorator
    {
        private int m_Id;
        private string m_Title;
        public FlowerMenuItem(int id, string title)
        {
            this.m_Id = id;
            this.m_Title = title;
        }

        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        public string Title
        {
            get
            {
                return m_Title;
            }
        }
    }
}
