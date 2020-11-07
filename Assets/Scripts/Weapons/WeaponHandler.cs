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
            var pos = Vector3.one * .5f;
            CurrentWeapon = w;
            weapon.transform.SetParent(transform);
            weapon.transform.localPosition = pos;
            // new Vector3(-.5f, 0)
            weapon.transform.LookAt(transform.position + pos + (transform.forward * 100f));
        }
        
    }
    #endregion
}
