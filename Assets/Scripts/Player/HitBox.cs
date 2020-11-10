using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player {

    [RequireComponent(typeof(BoxCollider))]
    public class HitBox : MonoBehaviour {

        #region Methods
        public event System.Action<GameObject> OnCollide;

        private void OnTriggerEnter(Collider c) {
            Collide(c.gameObject);
        }

        private void OnCollisionEnter(Collision c) {
            Collide(c.gameObject);
        }

        private void Collide(GameObject o) {
            OnCollide?.Invoke(o);
        }
        #endregion
    }
}
