using Assets.Scripts.Player;
using UnityEngine;
using System;

namespace Assets.Scripts.Weapons {
    #region Weapon Enums
    public enum Layer {
        Base = 1,
        Firearm = 2,
        Melee = 3,
    }
    #endregion

    [RequireComponent(typeof(Animator))]
    public class WeaponHandler : MonoBehaviour {
        protected static readonly Vector3 FirearmPosition = new Vector3(0.241f, -0.03f, 0.019f);
        protected static readonly Vector3 FirearmEulerRotation = new Vector3(-0.365f, 94.091f, 90.735f);

        #region Exposed Variables
        public Transform RightHand;
        public GameObject Weapon;
        #endregion

        #region Variables
        private Animator Animator;
        #endregion

        #region Properties
        public Weapon CurrentWeapon { get; protected set; } = null;
        public bool HasWeapon => CurrentWeapon;
        #endregion

        #region Methods
        protected virtual void Start() {
            Animator = GetComponent<Animator>();
            if (Weapon) Equip(Weapon);
        }

        protected virtual void Update() {
            /*if (Fire1 && CurrentWeapon) {
                CurrentWeapon.TryAttacking();
            }*/
        }

        public virtual bool Equip(GameObject weapon) {
            Weapon Weapon;
            try {
                Weapon = weapon.GetComponent<Weapon>();
                if (!Weapon) return false;
            } catch { return false; }

            if (HasWeapon) {
                if (CurrentWeapon is Firearm a) {
                    a.OnShoot -= OnShoot;
                }
                CurrentWeapon.gameObject.transform.SetParent(null);
                CurrentWeapon.IsPickedUp = false;
            }
            CurrentWeapon = Weapon;
            weapon.transform.SetParent(RightHand);
            ActivateLayer(Layer.Firearm);
            CurrentWeapon.IsPickedUp = true;
            weapon.transform.localPosition = FirearmPosition;
            weapon.transform.localEulerAngles = FirearmEulerRotation;

            if (Weapon is Firearm f) {
                f.OnShoot += OnShoot;
            }
            return true;
        }

        public void ActivateLayer(Layer layer) {
            void set(Layer a) {
                float w = Mathf.Clamp01((float)(a & layer));
                string s = a.ToString("G");
                int i = Animator.GetLayerIndex(s);
                Animator.SetLayerWeight(i, w);
            }

            set(Layer.Base);
            set(Layer.Firearm);
            // set(Layer.Melee);
        }

        protected virtual void OnShoot() {
        }
        #endregion
    }
}