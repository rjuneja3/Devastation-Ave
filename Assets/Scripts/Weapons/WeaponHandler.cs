using Assets.Scripts.Player;
using UnityEngine;
using System;
using Assets.Scripts.Factions;

namespace Assets.Scripts.Weapons {
    #region Weapon Enums
    public enum Layer {
        Base = 1,
        Firearm = 2,
        Melee = 3,
    }
    #endregion

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(FactionEntity))]
    public abstract class WeaponHandler : MonoBehaviour {
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
        public abstract Vector3 BulletOrigin { get; }
        public abstract Vector3 BulletDirection { get; }
        public FactionEntity Entity { get; private set; }
        #endregion

        #region Methods
        protected virtual void Start() {
            Animator = GetComponent<Animator>();
            Entity = GetComponent<FactionEntity>();
            if (Weapon) Equip(Weapon);
        }

        protected virtual void Update() { }

        public virtual bool Equip(GameObject weaponObject) {
            
            Weapon weapon;
            try {
                weapon = weaponObject.GetComponent<Weapon>();
                if (!weapon) return false;
            } catch { return false; }

            CurrentWeapon?.AttachToHandler(null); // removes old Weapon from entity
            weapon.AttachToHandler(this); // Appends new weapon to entity
            weaponObject.transform.localPosition = FirearmPosition;
            weaponObject.transform.localEulerAngles = FirearmEulerRotation;

            ActivateLayer(weapon is Firearm
                ? Layer.Firearm : Layer.Base);

            Weapon = weaponObject;
            CurrentWeapon = weapon;
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

        public virtual void OnShoot() { }

        public virtual void OnHit(GameObject o) { }
        #endregion
    }
}