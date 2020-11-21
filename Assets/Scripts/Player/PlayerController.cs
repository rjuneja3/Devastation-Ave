using Assets.Imported.Standard_Assets.Characters.FirstPersonCharacter.Scripts;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Factions;

namespace Assets.Scripts.Player {
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour {
        const float GET_VOLUME_FROM_MIXER = .5f;

        #region Exposed Variables
        public float Speed = 5f;
        public float SprintSpeed = 20f;
        public float JumpHeight = 5f;
        public AudioClip HurtSound;
        public Transform Torso;
        #endregion

        #region Variables
        private AudioSource Audio;
        private CharacterController CharacterController;
        private Rigidbody Rigidbody;
        private Animator Animator;
        private Health Health;
        private CollisionFlags CollisionFlags;
        private bool CanJump = true;
        #endregion

        #region Properties
        public float HorizontalAxis => Input.GetAxis("Horizontal");
        public float VerticalAxis => Input.GetAxis("Vertical");
        public float MouseX => Input.GetAxis("Mouse X");
        public float MouseY => Input.GetAxis("Mouse Y");
        public bool Fire1 => Input.GetButton("Fire1") && !Input.GetKey(KeyCode.LeftControl);
        public bool IsGrounded => CharacterController.isGrounded;
        #endregion

        #region Methods

        void Start() {
            //MouseLook.Init(transform, Camera.main.transform);
            Audio = GetComponent<AudioSource>();
            CharacterController = GetComponent<CharacterController>();
            Animator = GetComponent<Animator>();
            Health = GetComponent<Health>();
            Health.OnHealthChange += OnHealthChange;
            Health.OnDeath += OnDeath;
            // Rigidbody = GetComponent<Rigidbody>();
            //ActivateLayer(Layer.Firearm);
        }

        private void OnHealthChange(float prevAmount, float currentAmount) {
            print($"Health Decrease: {prevAmount} -> {currentAmount}");
            if (currentAmount < prevAmount) {
                Audio.PlayOneShot(HurtSound, GET_VOLUME_FROM_MIXER);
            }
        }

        private void OnDeath() {
            Invoke("ReloadScene", 5f);
        }

        private void ReloadScene() {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        void Update() {
            Move();
        }

        public void TriggerFire() {
            Animator.SetTrigger("Fire");
        }

        /*private void FixedUpdate() {
            //MouseLook.UpdateCursorLock();
        }*/

        private void Move() {
            if (Input.GetKey(KeyCode.Space)) {
                Animator.SetTrigger("Jump");
            }

            const float speedLimit = 1;

            float h = HorizontalAxis,
                v = VerticalAxis;


            float xSpeed = h * speedLimit;
            float zSpeed = v * speedLimit;
            if (Input.GetKey(KeyCode.LeftShift)) {
                v *= 2f;
                h *= 2f;
            }

            Animator.SetFloat("XSpeed", h);//, .25f, Time.deltaTime);
            Animator.SetFloat("ZSpeed", v);//, .25f, Time.deltaTime);

            Animator.SetFloat("Speed", 1);//, .25f, Time.deltaTime);

            if (v != 0 && h != 0) {
                FactionManager.ProduceNoise(Faction.Player, NoiseType.Walking, transform.position);
            }
        }
        #endregion
    }
}
