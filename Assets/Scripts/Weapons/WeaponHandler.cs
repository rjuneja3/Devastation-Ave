using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Weapons {
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
            /*if (_TestWeapon && !HasWeapon && Vector3.Distance(transform.position, _TestWeapon.transform.position) <= 1f) {
                PickUp(_TestWeapon);
            }*/

            if (Fire1 && CurrentWeapon) {
                CurrentWeapon.TryAttacking();
            }
        }

        public void PickUp(GameObject weapon) {
            Weapon Weapon;
            try {
                Weapon = weapon.GetComponent<Weapon>();
            } catch (System.Exception) { return; }

            if (Weapon) {
                if (CurrentWeapon) {
                    if (CurrentWeapon is Firearm a)
                        a.OnShoot -= PlayerController.TriggerFire;
                    CurrentWeapon.gameObject.transform.SetParent(null);
                    CurrentWeapon.IsPickedUp = false;
                }
                CurrentWeapon = Weapon;
                weapon.transform.SetParent(RightHand);
                weapon.transform.localPosition = FirearmPosition;
                PlayerController?.ActivateLayer(PlayerController.Layer.Firearm);
                CurrentWeapon.IsPickedUp = true;
                // new Vector3(-.5f, 0)
                weapon.transform.localEulerAngles = FirearmEulerRotation;

                if (Weapon is Firearm f) {
                    f.OnShoot += PlayerController.TriggerFire;
                }

                HudHandler.ClearPrompt();
            }
        }
        #endregion
    }
}