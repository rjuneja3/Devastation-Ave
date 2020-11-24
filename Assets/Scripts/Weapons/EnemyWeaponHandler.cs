using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enemy;
namespace Assets.Scripts.Weapons {
    /**
     * @author Brenton Hauth
     * @date 11/24/20
     * <summary>
     * </summary>
     */
    public class EnemyWeaponHandler : WeaponHandler {
        #region Exposed Variables
        #endregion

        #region Variables
        #endregion

        #region Properties
        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * </summary>
         */
        public override Vector3 BulletOrigin => throw new NotImplementedException();

        /**
         * @author Brenton Hauth
         * @date 11/20/20
         * <summary>
         * </summary>
         */
        public override Vector3 BulletDirection => throw new NotImplementedException();
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
        protected override void Update() {
        }
        #endregion
    }
}

