using UnityEngine;

/* ----------------------------------------
 * class to demonstrate how to trigger and apply and Explosion force to 
 * a character featuring Ragdoll physics
 */ 
namespace Assets.Imported.std_assets._Scripts
{
    public class DeadlyObject : MonoBehaviour 
    {
        // Float variable for the explosion's radius
        public float range = 2f;

        // Float variable for the explosion's force
        public float force = 2f;

        // Float variable for the explosion's Upwards modifier
        public float up = 4f;

        /* ----------------------------------------
	 * If a game object with the'Player' tag enters the Trigger collider, and 'active' is true, 
	 * activate character's ragdoll physics, apply explosion force to it, deactivate trigger and start
	 * coroutine to re-activate it.
	 */
        void OnTriggerEnter(Collider hit)
        {

            //print("deadly hit: " + hit.gameObject.tag);
        

            if (hit.CompareTag("Player")) {
                try {
                    // MovementController mc = GetComponent<MovementController>();
                    // mc.stopMovement();
                } catch (System.NullReferenceException) { }
                RagdollCharacter ragdollCharacter = hit.gameObject.GetComponent<RagdollCharacter>();
                ExplodePlayer(ragdollCharacter);
                //Destroy(gameObject);
            }
        }

        public void HitObject(Collider hit) { OnTriggerEnter(hit); }
	
        /**
	 * explode the Player's character
	 * - given the reference to its RagdollCharacter component
	 */
        private void ExplodePlayer(RagdollCharacter ragdollCharacter)
        {
            // Activate ragdoll physics on character through its RagdollCharacter component 
            ragdollCharacter.ActivateRagdoll();

            // Create Vector for the explosion's position
            Vector3 explosionPos = transform.position;

            // Get all colliders within explosion radius
            Collider[] colliders = Physics.OverlapSphere(explosionPos, range);
		
            // For each collider within explosion radius...
            foreach (Collider collider in colliders) {
                if (collider.GetComponent<Rigidbody>())
                    // IF collider object features a rigidbody component, add explosion force to it
                    collider.GetComponent<Rigidbody>().AddExplosionForce(force, explosionPos, range, up);
            }
        }

    }
}
