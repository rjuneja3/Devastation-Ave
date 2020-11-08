using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
    #region Exposed Variables
    public float Value = 100;
    #endregion

    #region Varibles
    #endregion

    #region Properties
    #endregion

    #region Methods
    public event Action OnDeath;

    void Start() {
    }

    void Update() {
    }
    #endregion
}
