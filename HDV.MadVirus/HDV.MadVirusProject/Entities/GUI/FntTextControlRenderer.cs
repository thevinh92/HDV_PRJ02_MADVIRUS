using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace HDV.MadVirus.Entities.GUI
{
    

    public class FntTextControlRenderer : Drawable2D
    {
        static int instances;

        [RequiredComponent]
        private FntTextControl m_FntTextControl;

        [RequiredComponent]
        private Transform2D m_Transform;

        private AddressMode samplerMode;

        public FntTextControlRenderer(Type layer, AddressMode samplerMode = AddressMode.LinearClamp)
            : base("FntTextControlRenderer" + instances++, layer)
        {
            this.m_FntTextControl = null;
            this.samplerMode = samplerMode;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.m_FntTextControl != null)
            {
                m_FntTextControl.Dispose();
            }
        }

        public override void Draw(TimeSpan gameTime)
        {
            if (this.m_Transform.GlobalOpacity > this.Delta)
            {
                float b = base.RenderManager.DebugLines ? this.DebugAlpha : this.m_Transform.GlobalOpacity;
                Color color = m_FntTextControl.Color * b;

                Matrix worldTransform = m_Transform.WorldTransform;

                //Dịch chuyển về gốc tọa độ (góc trên bên trái)
                Vector3 localTranslation = new Vector3(
                -m_Transform.Origin.X * m_Transform.Rectangle.Width,
                -m_Transform.Origin.Y * m_Transform.Rectangle.Height,
                0);

                var letterIter = m_FntTextControl.CharacterRenderCommands.GetEnumerator();
                while (letterIter.MoveNext())
                {
                    FntCharacterRenderCommand renderCommand = letterIter.Current;
                    Vector2 letterOrigin = Vector2.Zero;

                    //Tính toán ma trận dịch chuyển
                    Vector3 letterLocalTranslation = new Vector3(renderCommand.Position, 0); //Vector dịch chuyển địa phương của ký tự
                    letterLocalTranslation += localTranslation;

                    Matrix letterTranslationMatrix;
                    Matrix.CreateTranslation(ref letterLocalTranslation, out letterTranslationMatrix);

                    Matrix letterLocalTransform = Matrix.Identity;
                    Matrix.Multiply(ref letterLocalTransform, ref letterTranslationMatrix, out letterLocalTransform);

                    //Tính toán ma trận toàn cục của ký tự
                    Matrix letterWorldTransform;
                    Matrix.Multiply(ref letterLocalTransform, ref worldTransform, out letterWorldTransform);

                    layer.SpriteBatch.DrawVM(renderCommand.Texture2D, renderCommand.SourceRect, ref color, ref letterOrigin, m_Transform.Effect, ref letterWorldTransform, m_Transform.DrawOrder);
                }
            }
        }
    }
}
