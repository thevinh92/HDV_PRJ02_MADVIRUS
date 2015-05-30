using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace HDV.MadVirus.Entities.GUI.Controls
{
    public class ProgressBar : BaseDecorator
    {
        private float m_Percentage = 1.0f;
        private Sprite m_IndicatorSprite;

        public ProgressBar()
        {
            this.entity = CreateEntity();
        }

        private Entity CreateEntity()
        {
            Entity entity = new Entity().
                AddComponent(new Transform2D { 
                    Origin = new WaveEngine.Common.Math.Vector2(0.5f)
                }).
                AddComponent(new Sprite("Content/progress_bar/progress_bar_background.png")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));
            entity.EntityInitialized += entity_EntityInitialized;
            
            //Progress Bar Indicator
            var indicator = new Entity().
                AddComponent(new Transform2D { 
                    X = - 176,
                    Origin = new WaveEngine.Common.Math.Vector2(0f, 0.5f)
                }).
                AddComponent(new Sprite("Content/progress_bar/progress_bar_indicator.png")).
                AddComponent(new SpriteRenderer(DefaultLayers.Alpha));

            this.m_IndicatorSprite = indicator.FindComponent<Sprite>();
            entity.AddChild(indicator);

            //Add Behavior
            entity.AddComponent(new ProgressBarBehavior(this));

            return entity;
        }

        void entity_EntityInitialized(object sender, EventArgs e)
        {
            OnPercentageChanged();
        }

        public float Percentage
        {
            set
            {
                var temp = value;
                if (temp > 1.0f)
                    temp = 1.0f;
                else if (temp < 0)
                    temp = 0f;

                if (m_Percentage != temp)
                {
                    m_Percentage = temp;
                    OnPercentageChanged();
                }
            }
            get
            {
                return m_Percentage;
            }
        }

        private void OnPercentageChanged()
        {
            if (m_IndicatorSprite.Texture == null)
                return;

            int textureWidth = m_IndicatorSprite.Texture.Width;
            int textureHegith = m_IndicatorSprite.Texture.Height;

            int regionWidth = (int)((float)textureWidth * m_Percentage);

            m_IndicatorSprite.SourceRectangle = 
                new WaveEngine.Common.Math.Rectangle(0, 0, regionWidth, textureHegith);
        }

        public void StartAnimation(float fromPercentage, float toPercentage, 
            double inMilliseconds, Action completedCallback)
        {
            var behavior = Entity.FindComponent<ProgressBarBehavior>();
            behavior.StartAnimation(fromPercentage, toPercentage, inMilliseconds, completedCallback);
        }

        private class ProgressBarBehavior : Behavior
        {
            private bool m_IsAnimating;
            private ProgressBar m_ProgressBar;
            private float m_FromPercentage;
            private float m_ToPercentage;
            private double m_InMilliseconds;
            private double m_TimeElapsed;

            private Action m_CompletedCallback;

            public ProgressBarBehavior(ProgressBar progressBar)
            {
                this.m_ProgressBar = progressBar;
            }

            public void StartAnimation(float fromPercentage, float toPercentage, 
                double inMilliseconds, Action completedCallback)
            {
                if (m_IsAnimating)
                    return;

                this.m_FromPercentage = fromPercentage;
                this.m_ToPercentage = toPercentage;
                this.m_InMilliseconds = inMilliseconds;
                this.m_TimeElapsed = 0;

                this.m_IsAnimating = true;
                this.m_CompletedCallback = completedCallback;
            }

            protected override void Update(TimeSpan gameTime)
            {
                if (!m_IsAnimating)
                    return;

                m_TimeElapsed += gameTime.TotalMilliseconds;
                float percentage = (float)(m_FromPercentage + (m_ToPercentage - m_FromPercentage) * (m_TimeElapsed / m_InMilliseconds));
                m_ProgressBar.Percentage = percentage;

                if (percentage >= m_ToPercentage)
                {
                    this.m_IsAnimating = false;

                    if (m_CompletedCallback != null)
                        m_CompletedCallback();
                }
            }
        }
    }
}
