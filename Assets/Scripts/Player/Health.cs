using System;
using UnityEngine;

namespace Assets.Scripts.Player {
    public class Health : MonoBehaviour {
        #region Exposed Variables
        public float StartingHealth = 100;
        public bool DestroyOnDeath = true;
        #endregion

        #region Varibles
        private bool TriggeredFinish = false;
        private float m_CurrentHP = 0;
        #endregion

        #region Properties
        public float CurrentHP {
            get => m_CurrentHP;
            set {
                var prev = m_CurrentHP;
                m_CurrentHP = Mathf.Clamp(value, 0, StartingHealth);
                OnHealthChange?.Invoke(prev, m_CurrentHP);
                DeathCheck();
            }
        }

        public bool IsDead => CurrentHP == 0;
        #endregion

        #region Methods
        public event Action OnDeath;
        public event Action<float, float> OnHealthChange;

        private void Start() {
            Resurrect();
        }

        private void DeathCheck() {
            if (CurrentHP <= 0 && !TriggeredFinish) {
                TriggeredFinish = true;
                HandleDeath();
            }
        }

        private void HandleDeath() {
            OnDeath?.Invoke();
            OnDeath = null;
            if (DestroyOnDeath) {
                Destroy(gameObject);
            }
        }

        public void Resurrect() {
            CurrentHP = StartingHealth;
            TriggeredFinish = false;
        }
        #endregion
    }
}
