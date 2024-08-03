using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] private CharacterController controller;

    [Header("Walk/Run Options")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float runBuildup = 1f;
    [SerializeField] private KeyCode runKey;

    private float speed; 

    Vector3 velocity;
    private bool isGrounded;

    [Header("Jump Variables")]
    public float gravity = -9.81f;
    public float jumpHeight = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
         PlayerMove();
    }

    private void PlayerMove()
    {
        //jump check 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //Input Manager check 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //constant force applied while on the ground. This keeps the charcter from doing strange things on slopes
        if (isGrounded && velocity.y < 0)
            velocity.y = -2.0f;

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        //gravity 
        velocity.y += gravity * Time.deltaTime;

        //make the character run around
        controller.Move(velocity * Time.deltaTime);
        SetMovementSpeed();

    }

    //allows the player to walk/run
    private void SetMovementSpeed()
    {
        if (Input.GetKey(runKey))
        {
            speed = Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * runBuildup);
        }
        else
        {
            speed = Mathf.Lerp(walkSpeed, runSpeed, Time.deltaTime * runBuildup);
        }
    }
}
