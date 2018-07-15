using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour {

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private float speedInAir = 0.5f;
    
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private Vector3 jumpDirection = Vector3.zero;
    private Vector3 bodyRotation = Vector3.zero;
    private float speed = 0f;

    private float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SerializeField]
    private float jumpCD = 0.75f;
    private float jumpCD_timer = 0f;

    private int jumpCounter = 0;

    //states
    private bool onGround = false;
    private bool running = false;
    private bool jumping = false;

    private Rigidbody rb;




	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();

        jumpCD_timer = jumpCD;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Rotate();
        Move();

        //--TIMERS
        //jump
        if (jumpCD_timer <= 0)
        {
            jumpCD_timer = jumpCD;
        }
        else
        {
            if (jumpCD_timer < jumpCD)
            {
                jumpCD_timer -= Time.fixedDeltaTime;
            }
        }
    }

    public void Move() {

        if(IsOnGround())
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            running = true;
        } else
        {
            if (direction == Vector3.zero || jumpDirection == Vector3.zero) direction = jumpDirection;
            rb.MovePosition(rb.position + direction * speedInAir * speed * Time.fixedDeltaTime);
            running = false;
        }

        if(direction == Vector3.zero)
        {
            running = false;
        }
    }

    public void Rotate() {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(bodyRotation));
        if(cam != null)
        {
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }

    public bool Jump(float _speed, Vector3 _direction)
    {
        jumping = false;
        if (jumpCD_timer == jumpCD)
        {
            Vector3 altitude = new Vector3(0f, 1f, 0f);
            if (!IsOnGround() && jumpCounter == 1)
            {

                rb.velocity -= rb.velocity;
                //rb.velocity += speed * Vector3.up;
                rb.AddRelativeForce(_speed * altitude, ForceMode.Impulse);

                jumpCounter++;
                jumpCD_timer -= Time.fixedDeltaTime;
                onGround = false;
                jumpDirection = direction;
                jumping = true;
            }

            if (IsOnGround() || jumpCounter == 0)
            {
                //rb.velocity += speed * Vector3.up;
                rb.AddRelativeForce(_speed * altitude, ForceMode.Impulse);
               
                jumpCounter++;
                jumpCD_timer -= Time.fixedDeltaTime;
                onGround = false;
                jumpDirection = direction;
                jumping = true;
            }
        }
        return jumping;
    }

    private void OnCollisionEnter(Collision collision)
    {
        /*
        if (collision.gameObject.tag == "Ground")
        {
            jumpCounter = 0;
            onGround = true;
        }
        */
        foreach(ContactPoint cp in collision.contacts)
        {
            //ground
            if(cp.normal == Vector3.up)
            {
                jumpCounter = 0;
                onGround = true;
            }
            
        }
        
    }

    public void SetDirection(Vector3 _direction)
    {
        direction = _direction;
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }

    public void SetBodyRotation(Vector3 _rotation)
    {
        bodyRotation = _rotation;
    }

    public void SetCameraRotation(float _rotation)
    {
        cameraRotationX = _rotation;
    }

    public bool IsOnGround() { return onGround; }
    public bool IsRunning() { return running; }
    public bool IsJumping() { return jumping; }
}
