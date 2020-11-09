using UnityEngine;

/* ----------------------------------------
 * class to demonstrate how to apply an explosion force
 * to surrounding objects 
 */ 
namespace Assets.Imported.std_assets._Scripts
{
    public class Explode : MonoBehaviour {
        public AudioClip ExplodeClip;
        public GameObject ExplodeParticles;
        // Float variable for the radius of the area affected by the explosion
        public float radius = 20.0F;
        // Float variable for the intensity of the explosion
        public float power = 300.0F;
        // Float variable for upwards modifier of the explosion
        public float upwards = 3.0f;
        // Public float variable for time between throw and explosion
        public float timer = 6.0f;
        // Float variable for time limit before explosion  
        private float timeLimit;

        private bool calledExplode = false;

        private DeadlyObject deadlyObject;

        /* ----------------------------------------
	 * At start, set timer limit as current time plus 'timer' variable
	 */
        void Start() {
            //Set timer limit as current time plus 'timer' variable
            //timeLimit = Time.time + timer;
            //calledExplode = false;
            Invoke("Detonate", timer);
            deadlyObject = GetComponent<DeadlyObject>();

        }

        /* ----------------------------------------
	 * During Update, check if it's time for the explosion
	 */
        void Update() {
            /*if (!calledExplode) {
			Invoke("Detonate", timer);
			calledExplode = true;
		}*/
            //if (Time.time >= timeLimit)
            // IF current time is equal or greater than timer limit, THEN call Detonate function.
            //	Detonate ();
        }

        /* ----------------------------------------
	 * A function for applying explosion force to surrounding objects
	 */
        public void Detonate() {
            Instantiate(ExplodeParticles, transform.position, Quaternion.identity);
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(ExplodeClip);
            //this.gameObject.GetComponent<AudioSource>().Play();
            // Vector 3 variable for the origin of the explosion, set as the object's current position
            Vector3 explosionPos = transform.position;

            // List of colliders detected within a sphere positioned at the origin of the explosion, and radius set by user
            Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

            // For each Collider found in the list
            foreach (Collider col in colliders) {
                // Get Rigidbody component of game object
                Rigidbody rb = col.GetComponent<Rigidbody>();
                deadlyObject.HitObject(col);

                if (rb != null)
                    // IF game object features a rigidbody component, add explosion force passing variables set by user as parameters for strength, origin, radius and upwards modifier 
                    rb.AddExplosionForce(power, explosionPos, radius, upwards);

                // Destroy object
                Destroy (gameObject);

            }
        }

        /*IEnumerator MakeSound() {
        yield return new WaitFor
        
    }*/
    }
}
