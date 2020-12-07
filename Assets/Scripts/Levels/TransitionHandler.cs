using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Levels {
    public enum Level {
        Level1 = 1,
        // Names have spaces
        Level_2 = 2,
        Level_3 = 3
    }

    // [RequireComponent(typeof(Collider))]
    public class TransitionHandler : MonoBehaviour {
        #region Exposed Variables
        public Level NextLevel;
        #endregion

        #region Variables
        #endregion

        #region Properties
        public static TransitionHandler Self { get; private set; }
        #endregion

        #region Methods
        private void Start() {
        }

        private void Update() {
        }

        public static void LoadLevel(Level level) {
            string name = level.ToString("G").Replace('_', ' ');
            try {
                SceneManager.LoadScene(name);
            } finally { }
        }

        private void OnTriggerEnter(Collider c) => Collide(c.gameObject);
        private void OnCollisionEnter(Collision c) => Collide(c.gameObject);

        private void Collide(GameObject o) {
            if (o.tag == "Player") {
                // Trigger fade out
                LoadLevel(NextLevel);
            }
        }
        #endregion
    }
}

