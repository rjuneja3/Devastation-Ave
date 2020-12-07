using Assets.Scripts.Factions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Weapons {
    #region Firearm Enums
    /**
     * @author Brenton Hauth
     * @date 11/25/20
     * <summary>
     * Suggests what action should be taken depending on the amount of ammo
     * </summary>
     */
    [Flags]
    public enum ReloadFlag {
        None = 0,
        LowMag = 8,
        LowOnAmmo = 2,
        OutOfAmmo = 4,
    }
    #endregion

    /**
     * @author Brenton Hauth
     * @date 11/25/20
     * <summary>
     * Attached to all Firearm based weapons. Extends <c>Weapon</c> class.
     * </summary>
     * <see cref="Weapon" />
     */
    public class Firearm : Weapon {
        public const float BulletDelay = 4f / 60f; // ~4 frames.

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
        private bool CurrentlyReloading = false;
        private uint m_CurrentMag = 0;
        #endregion

        #region Properties
        public bool CanReload => !CurrentlyReloading && m_CurrentMag < MagSize && Ammo > 0u;

        public float CurrentAdsRatio { get; private set; }
        protected override bool CanAttack {
            get => base.CanAttack && CurrentMag > 0;
            set => base.CanAttack = value;
        }
        public uint CurrentMag {
            get => m_CurrentMag;
            private set {
                m_CurrentMag = value;
                /*if (Ammo == 0u) {
                    CurrentFlags = m_CurrentMag == 0u
                        ? ReloadFlag.OutOfAmmo
                        : ReloadFlag.LowOnAmmo;
                } else if ((m_CurrentMag / (float)MagSize) <= .3f) {
                    CurrentFlags |= ReloadFlag.CanReload;
                } else {
                    CurrentFlags = ReloadFlag.None;
                }
                CurrentHandler?.OnReload(CurrentFlags);*/
            }
        }

        public ReloadFlag CurrentFlags { get; private set; }
        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>Update the accuracy of the shot</summary>
         * <param name="ratio">0.0 means they are aiming, 1.0 means they are not aiming</param>
         */
        public void SetAdsRatio(float ratio) {
            CurrentAdsRatio = Mathf.Clamp01(ratio);
        }

        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>Start method called by Unity</summary>
         */
        protected override void Start() {
            base.Start();
            var sp = transform.Find("ShootPoint");
            if (sp) {
                ShootPoint = sp.gameObject;
                ShootPointLight = sp.GetComponent<Light>();
            }
            RefillMag();
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>Refils the mag</summary>
         */
        private void RefillMag() {
            uint amount = MagSize - CurrentMag;
            uint take = Math.Min(Ammo, amount);
            CurrentMag += take;
            Ammo -= take;
            CurrentlyReloading = false;
        }

        public void Reload() {
            CurrentlyReloading = true;
            Invoke("RefillMag", 1.5f);
        }

        /**
         * @author Brenton Hauth
         * @date 11/08/20
         * <summary>Creates light at the end of the barrel</summary>
         */
        private void MuzzleFlash() {
            ShootPointLight.intensity = 1;
            Invoke("EndMuzzleFlash", BulletDelay / 2f);
        }

        /**
         * @author Brenton Hauth
         * @date 11/08/20
         * <summary>Turns off light at the end of the barrel</summary>
         */
        private void EndMuzzleFlash() {
            ShootPointLight.intensity = 0;
        }

        /**
         * @author Brenton Hauth
         * @date 10/31/20
         * <summary>"Shoots" the firearm</summary>
         */
        public override void Attack() { // Shoot
            var bullet = new Bullet {
                Origin = CurrentHandler.BulletOrigin,
                Direction = CurrentHandler.BulletDirection,
                Accuracy = CurrentAdsRatio * HipFireRadius
            };

            CurrentMag--; // Subtracts a bullet from the mag

            MuzzleFlash();
            if (CurrentHandler) {
                CurrentHandler.OnShoot();
                if (CurrentHandler?.Entity) {
                    FactionManager.ProduceNoise(CurrentHandler.Entity.Faction, NoiseType.GunShot, transform.position);
                }
            }
            BulletQueue.Enqueue(bullet);
            Invoke("DelayBullet", BulletDelay);
        }

        /**
         * @author Brenton Hauth
         * @date 11/21/20
         * <summary>
         * Causes a delay in the bullet to make it appear as if it travels
         * </summary>
         */
        private void DelayBullet() {
            const int layer = FactionEntity.LANDSCAPE_AND_ENTITIES;
            Bullet b = BulletQueue.Dequeue();

            if (b.Accuracy != 0f)
                RoughDirection(ref b.Direction, b.Accuracy);

            if (Physics.Raycast(b.Origin, b.Direction, out var hit, Range, layer)) {
                Hit(hit.collider.gameObject);

                if (DebugMode)
                    Debug.DrawLine(b.Origin, hit.point, Color.green, 5f);

            } else if (DebugMode) {
                var point = b.Origin + (b.Direction * Range);
                Debug.DrawLine(b.Origin, point, Color.yellow, 5f);
            }
        }

        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>
         * Changes the direction of the bullet depending on hipfire & accuracy
         * </summary>
         */
        private static void RoughDirection(ref Vector3 direction, float hipfire) {
            // change calculation, not sure if correct
            direction.x += Random.Range(-hipfire, hipfire);
            direction.y += Random.Range(-hipfire, hipfire);
        }
        #endregion

        #region Bullet Struct
        /**
         * @author Brenton Hauth
         * @date 10/20/20
         * <summary>Holds the data to cast a ray bullet</summary>
         */
        public struct Bullet {
            public Vector3 Origin;
            public Vector3 Direction;
            public float Accuracy;
        }
        #endregion
    }
}
