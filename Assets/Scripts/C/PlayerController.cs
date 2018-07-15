using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerController : NetworkBehaviour {

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpSpeed = 1.5f;
    [SerializeField]
    private float sensitivity = 5f;
    [SerializeField]
    private float gravity = 10f;

    private PlayerMotor motor;
    private PlayerWeapon weapon;

    [SerializeField]
    private bool escapeMenu = false;

    [SerializeField]
    private Animator animator;


    // Use this for initialization
    void Start () {
        motor = GetComponent<PlayerMotor>();
        weapon = GetComponent<PlayerWeapon>();
        animator = transform.Find("Skin").GetComponent<Animator>();
        if(animator == null) { Debug.LogError("PlayerController : animator not found"); }
	}
	
	// Update is called once per frame
	void Update () {
        //Cursor Lock
        if(Input.GetButtonDown("Cancel"))
        {
            escapeMenu = !escapeMenu;

            if (escapeMenu && Cursor.lockState != CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        if (!escapeMenu && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }



        //Movement
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");
        
        if (!motor.IsOnGround())
        {
            xMove = 0;
            zMove = Mathf.Clamp(zMove, 0f, 1f);
            //Debug.Log(xMove + " " + zMove);
        }
        
        Vector3 horizontalMove = transform.right * xMove;
        Vector3 verticalMove = transform.forward * zMove;

        Vector3 direction = (horizontalMove + verticalMove).normalized;

        //if (!motor.IsOnGround() && (xMove != 0 || zMove != 0)) Debug.Log("x" + xMove + horizontalMove + " z" + zMove + verticalMove + " = " +direction);

        motor.SetSpeed(speed);
        motor.SetDirection(direction);

        //Rotation
        float yRot = Input.GetAxisRaw("Mouse X");
        float xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 bodyRotation = new Vector3(0f, yRot, 0f) * sensitivity;
        float cameraRotationX = xRot * sensitivity;

        motor.SetBodyRotation(bodyRotation);
        motor.SetCameraRotation(cameraRotationX);

        //Jump
        bool jump = Input.GetButton("Jump");
        bool jumpSucceed = false;
        if (jump)
        {
            horizontalMove = transform.right * Input.GetAxisRaw("Horizontal");
            verticalMove = transform.forward * Input.GetAxisRaw("Vertical");
            direction = (horizontalMove + verticalMove).normalized;
            Debug.Log(direction);
            jumpSucceed = motor.Jump(jumpSpeed, direction);
        }

        //Shoot
        if(Input.GetButtonDown("Fire1"))
        {
            weapon.Shoot();
        }

        //Animations
        animator.SetBool("isOnGround", motor.IsOnGround());
        animator.SetBool("isRunning", motor.IsRunning());
        if(jumpSucceed) animator.Play("HumanoidJumpForwardLeft");
    }
}
