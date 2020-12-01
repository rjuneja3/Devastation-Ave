using Assets.Scripts.Player;
using UnityEngine;
using System;
using Assets.Scripts.Factions;

namespace Assets.Scripts.Weapons {
    #region Weapon Enums
    /**
     * @author Brenton Hauth
     * @date 11/25/20
     * <summary>
     * Used to trigger different animation layers,
     * depending on what weapon they are holding
     * </summary>
     */
    public enum AnimationLayer {
        Base = 1,
        Firearm = 2,
        Melee = 4,
    }
    #endregion

    /**
     * @author Brenton Hauth
     * @date 10/25/20
     * <summary>
     * Handles all weapon interactions (both Melee & Firearm).
     * Is extended by <c>PlayerWeaponHandler</c> & <c>EnemyWeaponHandler</c>
     * </summary>
     * <see cref="PlayerWeaponHandler" />
     * <see cref="EnemyWeaponHandler" />
     */
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(FactionEntity))]
    public abstract class WeaponHandler : MonoBehaviour {

        #region Exposed Variables
        public Transform RightHand;
        public GameObject Weapon;
        #endregion

        #region Variables
        private Animator Animator;
        #endregion

        #region Properties
        public bool HasWeapon => CurrentWeapon;
        public bool ShouldReload { get; private set; }
        public abstract Vector3 BulletOrigin { get; }
        public abstract Vector3 BulletDirection { get; }
        public abstract Vector3 FirearmPosition { get; }
        public abstract Vector3 FirearmEulerRotation { get; }
        public Weapon CurrentWeapon { get; protected set; } = null;
        public FactionEntity Entity { get; private set; }
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 11/08/20
         * <summary>Start method called by Unity.</summary>
         */
        protected virtual void Start() {
            Animator = GetComponent<Animator>();
            Entity = GetComponent<FactionEntity>();
            if (Weapon) Equip(Instantiate(Weapon));
        }

        /**
         * @author Brenton Hauth
         * @date 11/08/20
         * <summary>Update function called by Unity.</summary>
         */
        protected virtual void Update() { }

        /**
         * @author Brenton Hauth
         * @date 11/18/20
         * <summary>Originally called "PickUp"; Equips the weapon to the entity.</summary>
         * <param name="weaponObject">The weapon to equip (requires Weapon component).</param>
         * <returns>whether or not the weapon was properly equiped</returns>
         */
        public virtual bool Equip(GameObject weaponObject) {          
            Weapon weapon;
            try {
                // Gets weapon component from weaponObject
                // TODO: Implement null check on 'weaponObject' to unequip weapon
                weapon = weaponObject.GetComponent<Weapon>();
                if (!weapon) return false;
            } catch { return false; }

            CurrentWeapon?.AttachToHandler(null); // removes old Weapon from entity
            weapon.AttachToHandler(this); // Appends new weapon to entity

            // Updates the weapon transform to the proper Position & Rotation
            weaponObject.transform.localPosition = FirearmPosition;
            weaponObject.transform.localEulerAngles = FirearmEulerRotation;

            ActivateLayer(weapon is Firearm
                ? AnimationLayer.Firearm : AnimationLayer.Base);

            Weapon = weaponObject;
            CurrentWeapon = weapon;
            return true;
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>
         * Activates the appropriate animation layer, depending on what weapon is equiped
         * and deactivate the other layers
         * </summary>
         * <param name="layer">The layer to activate</param>
         */
        public void ActivateLayer(AnimationLayer layer) {
            void set(AnimationLayer a) {
                float w = Mathf.Clamp01((float)(a & layer)); // Converts layer into 1f or 0f
                string s = a.ToString("G"); // Gets the literal name of the layer
                int i = Animator.GetLayerIndex(s); // Gets the layer index from the name
                Animator.SetLayerWeight(i, w); // sets the weight of the layer
            }

            // Runs code for each layer
            set(AnimationLayer.Base);
            set(AnimationLayer.Firearm);
            // set(Layer.Melee);
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>Called if the current weapon is a Firearm and it shoots</summary>
         */
        public virtual void OnShoot() { }
        // TODO: ^^^ potentially change to abstract method

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>Called if the current weapon hits an object</summary>
         * <param name="o">The object hit by the weapon (or bullet)</param>
         */
        public virtual void OnHit(GameObject o) { }
        // TODO: ^^^ potentially change to abstract method

        public virtual void OnReload(ReloadFlag suggestion) { }
        #endregion
    }
}