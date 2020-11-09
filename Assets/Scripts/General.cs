using UnityEngine;

namespace Assets.Scripts
{
    public class General : MonoBehaviour {

        private const float Speed = 10f;
        private const float Sensitivity = .5f;

        private Vector3 LastMousePosition = Vector3.zero;
        private bool FirstMove = true;

        private void Start() {
        
        }

        private void Update() {
            MovePosition();
            MoveMouse();
        }

        private void MovePosition() {
            var move = Speed * Time.deltaTime;
            var t = Camera.main.transform;

            if (Input.GetKey(KeyCode.W)) {
                transform.position += t.forward * move;
            } if (Input.GetKey(KeyCode.A)) {
                transform.position -= t.right * move;
            } if (Input.GetKey(KeyCode.S)) {
                transform.position -= t.forward * move;
            } if (Input.GetKey(KeyCode.D)) {
                transform.position += t.right * move;
            }
        }

        private void MoveMouse() {
            if (FirstMove) {
                FirstMove = false;
            } else {
                var delta = (Input.mousePosition - LastMousePosition) * Sensitivity;
                transform.Rotate(new Vector3(-delta.y, delta.x));
            }
            LastMousePosition = Input.mousePosition;

        }
    }
}
