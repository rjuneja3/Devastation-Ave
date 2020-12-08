using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Levels {
    /**
     * @author Brenton Hauth
     * @date 12/07/20
     * <summary>
     * Handles Transitions between scenes
     * </summary>
     */
    public class TransitionHandler : MonoBehaviour {

        /**
         * @author Brenton Hauth
         * @date 12/07/20
         * <summary>Loads next level from based on level</summary>
         */
        public static void LoadNextLevel() {
            // IMPORTANT: Can cause bug if scenes are not in order
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

            // May be better to do something like this instead:
            //   string name = SceneManager.GetActiveScene().name;
            //   name = $"Level {(char)(name[name.Length - 1] + 1)}";
            //   SceneManager.LoadScene(name);
        }

        private void OnTriggerEnter(Collider c) => Collide(c.gameObject);
        private void OnCollisionEnter(Collision c) => Collide(c.gameObject);

        /**
         * @author Brenton Hauth
         * @date 12/07/20
         * <summary>
         * Handles all collisions from Colliders
         * </summary>
         */
        private void Collide(GameObject o) {
            if (o.tag == "Player") {
                // TODO: Trigger fade out
                LoadNextLevel();
            }
        }
    }
}

