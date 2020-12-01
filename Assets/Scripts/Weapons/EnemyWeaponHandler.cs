using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enemy;
using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;
using Assets.Scripts.General;

namespace Assets.Scripts.Weapons {
    /**
     * @author Brenton Hauth
     * @date 11/24/20
     * <summary>
     * </summary>
     */
    [RequireComponent(typeof(FactionEntity))]
    public class EnemyWeaponHandler : WeaponHandler {
        #region Exposed Variables
        public float Accuracy = 1f;
        #endregion

        #region Variables
        private static readonly Vector3 EnemyFirearmPosition = new Vector3(0f, 0.174f, 0.063f);
        private static readonly Vector3 EnemyFirearmEulerRotation = new Vector3(-100.278f, 230.321f, -40.765f);
        #endregion

        #region Properties
        public override Vector3 FirearmPosition => EnemyFirearmPosition;
        public override Vector3 FirearmEulerRotation => EnemyFirearmEulerRotation;

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * </summary>
         */
        public override Vector3 BulletOrigin => Entity.Head.transform.position;

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * </summary>
         */
        public override Vector3 BulletDirection {
            get {
                var target = RoughTargetLocation(Accuracy);
                return target - BulletOrigin; // No need to normalize
            }
        }

        #endregion

        #region Methods
        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Start method called by Unity
         * </summary>
         */
        protected override void Start() {
            base.Start();
        }

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * Update method called by Unity
         * </summary>
         */
        protected override void Update() { }

        /**
         * @author Brenton Hauth
         * @date 11/26/20
         * <summary>
         * Creates a rough direction to the associated entity target
         * </summary>
         * <param name="accuracy">How accurate they are. (0 means they will never miss)</param>
         * <returns>A point near the target (depending on the accuracy)</returns>
         */
        private Vector3 RoughTargetLocation(float accuracy) {
            if (accuracy == 0) {
                return Entity.Target.Position + Vector3.up;
            }
            accuracy = Mathf.Abs(accuracy);
            return Entity.Target.Position + Vector3.up +
                VectorHelper.RandomVector3(-accuracy, accuracy);
        }

        /**
         * @author Brenton Hauth
         * @date 11/22/20
         * <summary>Called if the current weapon hits an object</summary>
         * <param name="o">The object hit by the weapon (or bullet)</param>
         */
        public override void OnHit(GameObject o) {
            Faction check = Faction.All ^ Entity.Faction;
            if (FactionManager.CheckCache(check, o.transform, out var entity, false)) {
                print($"{Entity} hit {entity}");
                entity.Health.CurrentHP -= CurrentWeapon.Damage * GameSettings.EnemyDamageMultiplier;
            }
        }
        #endregion
    }
}

