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
    private float m_CurrentAmount = 0;
    #endregion

    #region Properties
    public float CurrentAmount {
        get => m_CurrentAmount;
        set {
            var prev = m_CurrentAmount;
            m_CurrentAmount = Mathf.Clamp(prev - value, 0, StartingHealth);
            OnHealthChange?.Invoke(prev, m_CurrentAmount);
        }
    }
    #endregion

    #region Methods
    public event Action<GameObject> OnDeath;
    public event Action<float, float> OnHealthChange;

    void Start() {
        m_CurrentAmount = StartingHealth;
    }

        void Update() {
            if (CurrentAmount <= 0 && !TriggeredFinish) {
                TriggeredFinish = true;
                HandleDeath();
            }
        }

        void HandleDeath() {
            OnDeath?.Invoke(gameObject);
            if (DestroyOnDeath) {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
