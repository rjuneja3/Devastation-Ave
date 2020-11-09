using System.Diagnostics;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour {

    public enum Layer {
        Base = 1,
        Firearm = 2,
        Melee = 3,
    }

    #region Exposed Variables
    public float Speed = 5f;
    public float SprintSpeed = 20f;
    public float JumpHeight = 5f;
    #endregion

    #region Variables
    private CharacterController CharacterController;
    private Rigidbody Rigidbody;
    private Animator Animator;
    private MouseLook MouseLook;// = new MouseLook();
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

    private bool t_EquipMelee = false;
    public bool T_EquipMelee {
        get => t_EquipMelee;
        set {
            t_EquipMelee = value;
            ActivateLayer(value ? Layer.Firearm : Layer.Base);
            print($"EquipedFirearm {value}");
        }
    }

    #region Methods
    public void ActivateLayer(Layer layer) {

        void set(Layer a, float w) {
            var s = a.ToString("G");
            int i = Animator.GetLayerIndex(s);
            Animator.SetLayerWeight(i, w);
        }

        if (layer != Layer.Base) set(Layer.Base, 0);
        if (layer != Layer.Firearm) set(Layer.Firearm, 0);
        if (layer != Layer.Melee) set(Layer.Melee, 0);

        set(layer, 1f);
    }

    void Start() {
        //MouseLook.Init(transform, Camera.main.transform);
        CharacterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        // Rigidbody = GetComponent<Rigidbody>();
        //ActivateLayer(Layer.Firearm);
    }

    void Update() {
        //Look();

        if (Input.GetKeyDown(KeyCode.M)) {
            T_EquipMelee = !T_EquipMelee;
        }

        //if (IsGrounded) {
        Move();
        //}



        //if (IsGrounded) {
        /*float moveFactor = Speed * Time.deltaTime;

        Vector3 move = Vector3.zero;
        move.x = HorizontalAxis * moveFactor;
        move.z = VerticalAxis * moveFactor;
        //transform.position += move;

        CharacterController.Move(move);*/
        //print(IsGrounded + " : " + HorizontalAxis + ", " + VerticalAxis);

        /*if (Fire1) { // TODO: Need to match actual fire rate
            Animator.SetTrigger("Fire");
        }*/
    }

    public void TriggerFire() {
        Animator.SetTrigger("Fire");
    }

    private void FixedUpdate() {
        //MouseLook.UpdateCursorLock();
    }

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

        /*if (CanJump && Input.GetKeyDown(KeyCode.Space)) {
            //Animator.SetBool("Jump", true);
            //CanJump = false;
            var rb = CharacterController.attachedRigidbody;
            if (rb) rb.AddForce(Vector3.up * 1000, ForceMode.Impulse);
            else print("no rb!!!");
        }*/

        Animator.SetFloat("XSpeed", h);//, .25f, Time.deltaTime);
        Animator.SetFloat("ZSpeed", v);//, .25f, Time.deltaTime);

        Animator.SetFloat("Speed", 1);//, .25f, Time.deltaTime);
    }


    private void Look() {
        MouseLook.LookRotation(transform, Camera.main.transform);
    }


    /*public void OnCollisionEnter(Collision c) {
        Collide(c.gameObject);
    }

    public void OnTriggerEnter(Collider c) {
        Collide(c.gameObject);
    }

    private void Collide(GameObject g) {
        if (g.tag == "Ground") {
            if (!CanJump) {
                CanJump = true;
                Animator.SetBool("Jump", false);
            }
            print("Hit ground");
        }
    }*/


    /*private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (CollisionFlags == CollisionFlags.Below) {
            return;
        }

        if (body == null || body.isKinematic) {
            return;
        }
        body.AddForceAtPosition(CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }*/

    /*void OnAnimatorMove() {
        Vector3 deltaPosition = Animator.deltaPosition;
        print(deltaPosition);

        if (IsGrounded) {
            VelocityX = CharacterController.velocity.x;
            VelocityZ = CharacterController.velocity.z;
        } else {
            deltaPosition.x = VelocityX * Time.deltaTime;
            deltaPosition.z = VelocityZ * Time.deltaTime;
        }

        deltaPosition.y = verticalSpeed * Time.deltaTime;
        CharacterController.Move(deltaPosition);
        verticalSpeed += Physics.gravity.y * Time.deltaTime;

        if (CharacterController.collisionFlags.HasFlag(CollisionFlags.Below)) {
            verticalSpeed = 0;
        }
    }*/

    //void OnAnimatorMove() { }
    #endregion
}
