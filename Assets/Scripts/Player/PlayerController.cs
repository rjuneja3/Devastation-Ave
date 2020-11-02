using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Exposed Variables
    public float Speed = 10f;
    #endregion

    #region Variables
    private CharacterController CharacterController;
    // private Animator Animator;
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
        CharacterController = GetComponent<CharacterController>();
        // Animator = GetComponent<Animator>();
    }
    
    void Update() {
        //if (IsGrounded) {
        /*float moveFactor = Speed * Time.deltaTime;

        Vector3 move = Vector3.zero;
        move.x = HorizontalAxis * moveFactor;
        move.z = VerticalAxis * moveFactor;
        //transform.position += move;

        CharacterController.Move(move);*/
        //print(IsGrounded + " : " + HorizontalAxis + ", " + VerticalAxis);

        if (Fire1) {
            
        }
    }
    #endregion
}
