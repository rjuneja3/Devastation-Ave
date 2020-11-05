using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class Weapon : MonoBehaviour {
    #region Exposed Variables
    public float RateOfAttack = .5f;
    public float Range = 100f;
    public float Damage = 10f;
    public bool DebugMode = true;
    #endregion

    #region Variables
    protected bool CanAttack = true;
    protected AudioSource AudioSource;
    #endregion

    #region Properties
    #endregion

    #region Methods
    public event Action<GameObject> OnHit;

    protected virtual void Start() {
        AudioSource = GetComponent<AudioSource>();
    }

    private void SetAttackFlag() => CanAttack = true;
    
    public void TryAttacking() {
        if (CanAttack) {
            CanAttack = false;
            Invoke("SetAttackFlag", RateOfAttack);
        } else return;

        AudioSource.Play();

        Attack();
    }

    public virtual void Hit(GameObject o) {
        OnHit?.Invoke(o);
    }

    public abstract void Attack();
    #endregion
}
