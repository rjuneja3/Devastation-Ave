using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Assets.Scripts.Enemy;

namespace Assets.Scripts.Weapons {
    public class EnemyWeaponHandler : WeaponHandler {
        #region Exposed Variables
        #endregion

        #region Variables
        #endregion

        #region Properties
        public override Vector3 BulletOrigin => throw new NotImplementedException();
        public override Vector3 BulletDirection => throw new NotImplementedException();
        #endregion

        #region Methods
        protected override void Start() {
            base.Start();
        }

        protected override void Update() {
        }
        #endregion
    }
}

