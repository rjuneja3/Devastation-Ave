using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Animate {

        public enum Layer {
            Base = 1,
            Firearm = 2,
            Melee = 3,
        }

        private Animator m_Animator;
        private Layer Current = 0;

        public Animate(Animator a) {
            m_Animator = a;
        }

        public void Activate(Layer layer) {
            if (Current == layer) return;

            void set(Layer a, float w) {
                var s = a.ToString("G");
                int i = m_Animator.GetLayerIndex(s);
                m_Animator.SetLayerWeight(i, w);
            }

            if (layer != Layer.Base) set(Layer.Base, 0);
            if (layer != Layer.Firearm) set(Layer.Firearm, 0);
            if (layer != Layer.Melee) set(Layer.Melee, 0);

            set(layer, 1f);
            Current = layer;
        }
    }
}
