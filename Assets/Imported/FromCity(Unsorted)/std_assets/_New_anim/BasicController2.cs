using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BasicController2 : MonoBehaviour {


    public GameObject Bullet;
    
    public float MoveSpeed = 200f;

    public float transitionTime = 0.25f;

    private float speedLimit = 2f;

    public bool moveDiagonally = true;
    public bool mouseRotate = true;
    public bool keyboardRotate = true;


    private bool
        threwObject = false,
        IsSprinting = false,
        FullAuto = false,
        Reloaded = true;

    
    private Animator animator;
    private CharacterController characterController;
    //private ThrowObject throwObjectController;
    public GameObject Rifle { get; set; }
    private Vector3 OriginRiflePos;
    private Quaternion OriginRifleRot;
    private MouseAim Aim;

    private bool hrifle = false;
    /*public bool HasRifle {get{return hrifle;}
        set {hrifle=value;
            if (hrifle) {
                try {
                    Rifle = throwObjectController.PlayerRightHand.Find("M40A3 Rifle_prefab").gameObject;
                } catch (System.NullReferenceException) {
                    Rifle = throwObjectController.PlayerRightHand.Find("rifle").gameObject;
                }
                
                throwObjectController.PlayerRifle = Rifle;
                throwObjectController.OriginRiflePos = Rifle.transform.position;
                Rifle.transform.parent = throwObjectController.PlayerLeftHand;
                MouseAim ma = GetComponent<MouseAim>();
                ma.weapon = Rifle.transform;
                ma.ShootPoint = ma.weapon.Find("ShootPoint");
            }
        } }*/
    void Start() {
        //HasRifle = false;
        characterController = GetComponent<CharacterController>();
        //throwObjectController = GetComponent<ThrowObject>();
        animator = GetComponent<Animator>();
        Aim = GetComponent<MouseAim>();
    }

    void Update() {
        if (characterController.isGrounded) {
            if (Input.GetKeyDown(KeyCode.Q))
                FullAuto = !FullAuto;

            if (Input.GetKey(KeyCode.X)) {
                speedLimit = 0.9f;// .75f * MoveSpeed;
                IsSprinting = false;
            } else if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift)) {
                speedLimit = 2f; //5f * MoveSpeed;
                print("sprint");
            } else {
                speedLimit =   1.5f;// * MoveSpeed;
            }
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            //IsSprinting = !(h == 0 && v == 0);
            float xSpeed = h * speedLimit;
            float zSpeed = v * speedLimit;

            float speed = Mathf.Sqrt((h * h) + (v * v));
            //print("speed " + speed);
            if (v != 0 && !moveDiagonally) xSpeed = 0f;

            if (v != 0 && keyboardRotate) {
                this.transform.Rotate(Vector3.up * h, Space.World);
            }

            if (mouseRotate) {
                this.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Mathf.Sign(v), Space.World);
            }

            animator.SetFloat("zSpeed", zSpeed, transitionTime, Time.deltaTime);
            animator.SetFloat("xSpeed", xSpeed, transitionTime, Time.deltaTime);
            animator.SetFloat("Speed", speed, transitionTime, Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.F) && !threwObject) {
                threwObject = true;
                //if (HasRifle) {
                    //throwObjectController.OriginRiflePos = Rifle.transform.position;
                    //Rifle.transform.parent = throwObjectController.PlayerLeftHand;

                //}
                
                animator.SetBool("Grenade", threwObject);
                
            } else if (threwObject) {
                threwObject = false;
                animator.SetBool("Grenade", threwObject);
                //Invoke("RePositionRifle", 3f);
            }
            if (FullAuto) {
                if (Input.GetButtonDown("Fire1")) animator.SetBool("Fire", true);
                if (Input.GetButton("Fire1")) {
                    if (Reloaded) {
                        Reloaded = false;
                        //if (HasRifle) Aim.FireBullet(Bullet);
                        Invoke("Reload", 0.2f);

                    }
                    
                }
                if (Input.GetButtonUp("Fire1")) { animator.SetBool("Fire", false); }
            } else {
                if (Input.GetButtonDown("Fire1")) {
                    animator.SetBool("Fire", true);
                    //if (HasRifle) Aim.FireBullet(Bullet);
                }
                if (Input.GetButtonUp("Fire1")) { animator.SetBool("Fire", false); }
            }
            
        }
    }

    void Reload() { Reloaded = true; }
}
