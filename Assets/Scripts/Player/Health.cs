using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    #region Exposed Variables
    public float StartingHealth = 100;
    public bool DestroyOnDeath = true;
    #endregion

    #region Varibles
    private bool TriggeredFinish = false;
    #endregion

    #region Properties
    public float CurrentAmount { get; set; }
    #endregion

    #region Methods
    public event Action<GameObject> OnDeath;

    void Start() {
        CurrentAmount = StartingHealth;
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
