using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Weapons {
    [RequireComponent(typeof(PlayerController))]
    public class PlayerWeaponHandler : WeaponHandler {
        #region Exposed Variables
        #endregion

        #region Variables
        private PlayerController PlayerController;
        #endregion

        #region Properties
        public bool Fire1 => Input.GetButton("Fire1");
        #endregion

        #region Methods
        protected override void Start() {
            base.Start();
            PlayerController = GetComponent<PlayerController>();
        }

        protected override void Update() {
            if (Fire1 && HasWeapon) {
                CurrentWeapon.TryAttacking();
            }
        }

        protected override void OnShoot() {
            base.OnShoot();
            PlayerController.TriggerFire();
        }

        public override bool Equip(GameObject weapon) {
            if (base.Equip(weapon)) {
                HudHandler.ClearPrompt();
                return true;
            }
            return false;
        }
        #endregion
    }
}