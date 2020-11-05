using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {

    #region Exposed Variables
    #endregion

    #region Variables
    private GameObject _TestWeapon;
    #endregion

    #region Properties
    public Weapon CurrentWeapon { get; private set; } = null;
    public float CurrentRateOfAttack { get; private set; } = /* for testing */ .5f;
    public bool HasWeapon => CurrentWeapon;
    public float MouseX => Input.GetAxis("Mouse X");
    public float MouseY => Input.GetAxis("Mouse Y");
    public bool Fire1 => Input.GetButton("Fire1");
    #endregion

    #region Methods
    void Start() {
        _TestWeapon = GameObject.FindGameObjectWithTag("Weapon");
        print("TEST WEAPON: " + _TestWeapon);
    }

    void Update() {
        if (!HasWeapon && Vector3.Distance(transform.position, _TestWeapon.transform.position) <= 1f) {
            PickUp(_TestWeapon);
        }

        if (Fire1 && CurrentWeapon) {
            CurrentWeapon.TryAttacking();
        }
    }

    private void PickUp(GameObject weapon) {
        var w = weapon.GetComponent<Weapon>();

        if (w) {
            CurrentWeapon = w;
            weapon.transform.SetParent(transform);
            weapon.transform.localPosition = Vector3.forward;
        }
        
    }
    #endregion
}
