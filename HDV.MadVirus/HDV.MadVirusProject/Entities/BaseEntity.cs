using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WaveEngine.Framework;

namespace HDV.MadVirus.Entities
{
    /// <summary>
    /// Đây là Class Wrapper một Entity của Wave Engine
    /// </summary>
    public abstract class BaseEntity
    {
        private Entity m_Entity; 

        public BaseEntity()
        {
            this.m_Entity = CreateEntity();
        }

        protected abstract Entity CreateEntity();

        public Entity Entity
        {
            get
            {
                return m_Entity;
            }
        }

        public static explicit operator Entity(BaseEntity baseEntity)
        {
            return baseEntity.m_Entity;
        }
    }
}
