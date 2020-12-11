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
        private e.LineDrawer drawer;

        public static MultiplayerWeaponHandler Local { get; internal set; }

        protected override void Start() {
            drawer = new e.LineDrawer();
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
            var pos = Camera.main.transform.position;
            drawer.DrawLineInGameView(pos,
                pos + (Camera.main.transform.forward * 100f), Color.yellow);
            CurrentWeapon?.PlaySound();
        }

        public override void OnHit(GameObject o) {
            //if (o.layer == )
            var identity = o.transform.GetComponent<NetworkIdentity>();
            if (identity) {
                Debug.Log($"OnHit() -> About to call CmdHit on {identity.netId.Value}");
                MultiplayerController.Local.CmdHit(identity.netId, CurrentWeapon.Damage);
            } else {
                Debug.LogWarning("OnHit() -> Could not call");
            }
        }
    }


    namespace e {
        public struct LineDrawer {
            private LineRenderer lineRenderer;
            private float lineSize;

            public LineDrawer(float lineSize = .02f) {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                //Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }

            private void init(float lineSize = .02f) {
                if (lineRenderer == null) {
                    GameObject lineObj = new GameObject("LineObj");
                    lineRenderer = lineObj.AddComponent<LineRenderer>();
                    //Particles/Additive
                    lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                    this.lineSize = lineSize;
                }
            }

            //Draws lines through the provided vertices
            public void DrawLineInGameView(Vector3 start, Vector3 end, Color color) {
                if (!lineRenderer) {
                    init();
                }

                //Set color
                lineRenderer.startColor = color;
                lineRenderer.endColor = color;

                //Set width
                lineRenderer.startWidth = lineSize;
                lineRenderer.endWidth = lineSize;

                //Set line count which is 2
                lineRenderer.positionCount = 2;

                //Set the postion of both two lines
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }

            public void Destroy() {
                if (lineRenderer != null) {
                    UnityEngine.Object.Destroy(lineRenderer.gameObject);
                }
            }
        }
    }
}
#pragma warning restore CS0618 // Type or member is obsolete
