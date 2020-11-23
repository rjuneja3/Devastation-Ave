using Assets.Scripts.Factions;
using System.Collections.Generic;
using UnityEngine;
using Action = System.Action;

namespace Assets.Scripts.Weapons
{
    public class Firearm : Weapon {
        public const float BulletDelay = 4f / 60f; // ~4 frames. Call of Duty does this.

        #region Exposed Variables
        public float HipFireRadius = 60f;
        public uint MagSize = 30;
        public uint Ammo = 0;
        public uint BulletCount = 1; // if it's a shotgun, change to 8
        #endregion

        #region Variables
        private Queue<Bullet> BulletQueue = new Queue<Bullet>();
        private GameObject ShootPoint;
        private Light ShootPointLight;
        #endregion

        #region Properties
        public float CurrentAdsRatio { get; private set; }
        #endregion

        #region Methods
        /// <param name="ratio">0.0 means they are aiming, 1.0 means they are not aiming</param>
        public void SetAdsRatio(float ratio) {
            CurrentAdsRatio = Mathf.Clamp01(ratio);
        }

        protected override void Start() {
            base.Start();
            var sp = transform.Find("ShootPoint");
            if (sp) {
                ShootPoint = sp.gameObject;
                ShootPointLight = sp.GetComponent<Light>();
            }
        }

        private void MuzzleFlash() {
            ShootPointLight.intensity = 1;
            Invoke("EndMuzzleFlash", BulletDelay / 2f);
        }

        private void EndMuzzleFlash() {
            ShootPointLight.intensity = 0;
        }

        public override void Attack() { // Shoot
            var bullet = new Bullet {
                Origin = CurrentHandler.BulletOrigin,
                Direction = CurrentHandler.BulletDirection,
                Accuracy = CurrentAdsRatio * HipFireRadius
            };

            MuzzleFlash();
            if (CurrentHandler) {
                CurrentHandler.OnShoot();
                FactionManager.ProduceNoise(CurrentHandler.Entity.Faction, NoiseType.GunShot, transform.position);
            }
            BulletQueue.Enqueue(bullet);
            Invoke("DelayBullet", BulletDelay);
        }

        private void DelayBullet() {
            Bullet b = BulletQueue.Dequeue();

            if (b.Accuracy != 0f)
                RoughDirection(ref b.Direction, b.Accuracy);

            if (Physics.Raycast(b.Origin, b.Direction, out var hit, Range)) {
                Hit(hit.collider.gameObject);

                if (DebugMode)
                    Debug.DrawLine(b.Origin, hit.point, Color.green, 5f);

            } else if (DebugMode) {
                var point = b.Origin + (b.Direction * Range);
                Debug.DrawLine(b.Origin, point, Color.yellow, 5f);
            }
        }

        private static void RoughDirection(ref Vector3 direction, float hipfire) {
            // change calculation, not sure if correct
            direction.x += Random.Range(-hipfire, hipfire);
            direction.y += Random.Range(-hipfire, hipfire);
        }
        #endregion

        public struct Bullet {
            public Vector3 Origin;
            public Vector3 Direction;
            public float Accuracy;
        }
    }
}
