using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Weapons;
using UnityEngine.Networking;

#pragma warning disable CS0618 // Type or member is obsolete
namespace Assets.Scripts.Multiplayer {
    [RequireComponent(typeof(MultiplayerController))]
    public class MultiplayerWeaponHandler : PlayerWeaponHandler {

        private MultiplayerController Controller;

        public static MultiplayerWeaponHandler Local { get; internal set; }

        protected override void Start() {
            Controller = GetComponent<MultiplayerController>();
            Animator = GetComponent<Animator>();
            if (Weapon) OutsideEquip(Instantiate(Weapon));
        }

        //protected override void Update() { }

        public override bool Equip(GameObject weapon) {
            var identity = GetComponent<NetworkIdentity>();
            if (!identity) {
                Debug.LogWarning($"{weapon.name} did not have an Identity");
                return false;
            }

            if (base.Equip(weapon)) {
                MultiplayerController.Local.CmdPickUp(
                    MultiplayerController.Local.netId,
                    identity.netId);
                return true;
            }
            return false;
        }

        public bool OutsideEquip(GameObject weapon) {
            if (base.Equip(weapon)) {
                Debug.Log($"Outside equiped: {weapon.name}");
                return true;
            } else {
                Debug.LogWarning($"Unable to Outside equiped: {weapon.name}");
                return false;
            }
        }

        public override void OnShoot() {
            MultiplayerController.Local.CmdSendMessageToServer(
                MultiplayerController.Local.netId, "TriggerFire");
        }

        public void TriggerFire() {
            Animator.SetTrigger("Fire");
            CurrentWeapon?.PlaySound();
        }

        public override void OnHit(GameObject o) {
            var identity = o.transform.GetComponent<NetworkIdentity>();
            if (identity) {
                MultiplayerController.Local.CmdHit(identity.netId, CurrentWeapon.Damage);
            }
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
