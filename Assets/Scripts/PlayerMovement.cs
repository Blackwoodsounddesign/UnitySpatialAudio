using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Controller")]
    [SerializeField] public CharacterController controller;

    [Header("Walk/Run Options")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float runBuildup = 1f;
    [SerializeField] private KeyCode runKey;

    private float speed; 

    Vector3 velocity;
    private bool isGrounded;

    [Header("Jump Variables")]
    [SerializeField] public float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] public float jumpHeight = 5f;

    [Header("Slopes")]
    [SerializeField] private float slopeForceRayLength = 1.5f;
    [SerializeField] private float slopeForce = 1f;

    [Header("Run Variables")]
    [SerializeField] private float runRate = 0.3f;
    [SerializeField] private float walkRate = 0.5f;
    [SerializeField] private float stepCoolDown;

    [Header("Footsteps")]
    [SerializeField] private bool useFootsteps;
    [SerializeField] private AudioSource footStepsPlacement;
    [SerializeField] private AudioClip[] footStep;

    [Header("Pause Menu")]
    [SerializeField] private bool usePauseMenu;
    [SerializeField] GameObject pauseMenu;
    public MouseLook player;
    private bool pausing = false;
    private bool canMove = true; 
    
    private void Start()
    {
        if(usePauseMenu)
            pauseMenu.SetActive(false);

        player = FindObjectOfType<MouseLook>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        PauseMenu();
        if (canMove)
        {
            PlayerMove();
            FootStepPlayer();
        }
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
        setMovementSpeed();

        //some more slope nonsense 
        if ((z != 0 || x != 0) && OnSlope())
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
    }

    private bool OnSlope()
    {
        if (!isGrounded)
            return false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength))
            return true;
       return false;
    }

    //allows the player to walk/run
    private void setMovementSpeed()
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

    private void FootStepPlayer()
    {
        if (!useFootsteps)
            return;

        stepCoolDown -= Time.deltaTime;
        if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown < 0f && isGrounded)
        {
            int clip = Random.Range(0, footStep.Length);
            footStepsPlacement.pitch = 1f + Random.Range(-0.2f, 0.2f);
            footStepsPlacement.PlayOneShot(footStep[clip], 0.9f);
            if(Input.GetKey(runKey))
                stepCoolDown = runRate;
            else
                stepCoolDown = walkRate;
        }
    }

    private void PauseMenu()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !pausing && usePauseMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pauseMenu.SetActive(true);
            pausing = true;
            player.enabled = false;
            canMove = false;
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && pausing)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pauseMenu.SetActive(false);
            pausing = false;
            player.enabled = true;
            canMove = true;
        }
    }

    public void BackToGame()
    {
        if (!useFootsteps)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        pauseMenu.SetActive(false);
        pausing = false;
        player.enabled = true;
        canMove = true;
    }
}
