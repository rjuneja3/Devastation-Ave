using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour {
    private static readonly Vector3 FirearmPosition = new Vector3(0.241f, -0.03f, 0.019f);
    private static readonly Vector3 FirearmEulerRotation = new Vector3(-0.365f, 94.091f, 90.735f);

    #region Exposed Variables
    public Transform RightHand;
    #endregion

    #region Variables
    private GameObject _TestWeapon;
    private PlayerController PlayerController;
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

        PlayerController = GetComponent<PlayerController>();
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
            weapon.transform.SetParent(RightHand);
            weapon.transform.localPosition = FirearmPosition;
            PlayerController?.ActivateLayer(PlayerController.Layer.Firearm);
            // new Vector3(-.5f, 0)
            weapon.transform.localEulerAngles = FirearmEulerRotation;
        }
        
    }
    #endregion
}
