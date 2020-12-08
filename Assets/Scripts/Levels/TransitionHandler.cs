using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Levels {
    
    // [RequireComponent(typeof(Collider))]
    public class TransitionHandler : MonoBehaviour {
   
        public static void LoadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void OnTriggerEnter(Collider c) => Collide(c.gameObject);
        private void OnCollisionEnter(Collision c) => Collide(c.gameObject);

        private void Collide(GameObject o) {
            if (o.tag == "Player") {
                // Trigger fade out
                LoadLevel();
            }
        }
    }
}

