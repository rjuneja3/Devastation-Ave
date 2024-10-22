﻿using Assets.Scripts.Factions;
using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Weapons {
    /**
     * @author Brenton Hauth
     * @date 11/18/20
     * <summary>
     * Handles all weapon interactions for the Player.
     * Extends <c>WeaponHandler</c>.
     * </summary>
     * <see cref="WeaponHandler" />
     */
    // [RequireComponent(typeof(PlayerController))]
    public class PlayerWeaponHandler : WeaponHandler {
        
        #region Exposed Variables
        #endregion

        #region Variables
        public static readonly Vector3 PlayerFirearmPosition = new Vector3(0.2f, -0.126f, 0.022f);
        public static readonly Vector3 PlayerFirearmEulerRotation = new Vector3(23.388f, 101.565f, 67.867f);
        private PlayerController PlayerController;
        #endregion

        #region Properties
        public bool Fire1 => Input.GetButton("Fire1");
        public override Vector3 BulletOrigin => Cam.transform.position;
        public override Vector3 BulletDirection => Cam.transform.forward;
        public override Vector3 FirearmPosition => PlayerFirearmPosition;
        public override Vector3 FirearmEulerRotation => PlayerFirearmEulerRotation;
        private Camera Cam;
        #endregion

        #region Methods
        protected override void Start() {
            base.Start();
            PlayerController = GetComponent<PlayerController>();
            Cam = transform.GetComponentInChildren<Camera>();
        }

        protected override void Update() {
            if (!HasWeapon) return;
            if (Fire1) {
                CurrentWeapon.TryAttacking();
            } else if (Input.GetKeyDown(KeyCode.R)) {
                if (CurrentWeapon is Firearm f && f.CanReload) {
                    f.Reload();
                }
            }
        }

        public override void OnShoot() {
            PlayerController?.TriggerFire();
        }

        public override bool Equip(GameObject weapon) {
            if (base.Equip(weapon)) {
                HudHandler.ClearPrompt();
                return true;
            }
            return false;
        }

        public override void OnHit(GameObject o) {
            base.OnHit(o);
            if (o.tag == "Enemy") {
                var health = o.GetComponent<Health>();
                health.CurrentHP -= Math.Abs(CurrentWeapon.Damage);
            }

            print($"Hit: {o.name} [{o.tag}]");
        }

        public override void OnReload(ReloadFlag suggestion) {
            HudHandler.Prompt(KeyCode.R, "to Reload");
        }
        #endregion
    }
}