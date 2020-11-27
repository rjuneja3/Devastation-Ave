using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enemy;
using Assets.Scripts.Factions;
using Assets.Scripts.Helpers;

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
                var target = RoughDirectionToTarget(Accuracy);
                return target - BulletOrigin;
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
         */
        private Vector3 RoughDirectionToTarget(float accuracy) {
            if (accuracy == 0) {
                return Entity.Target.Position + Vector3.up;
            }
            accuracy = Mathf.Abs(accuracy);
            return Entity.Target.Position + Vector3.up +
                VectorHelper.RandomVector3(-accuracy, accuracy);
        }

        public override void OnHit(GameObject o) {
            Faction check = Faction.All ^ Entity.Faction;
            if (FactionManager.CheckCache(check, o.transform, out var entity, false)) {
                print($"{Entity} hit {entity}");
                entity.Health.CurrentHP -= CurrentWeapon.Damage;
            }
        }
        #endregion
    }
}

