using UnityEngine;

public class MouseLook : MonoBehaviour
{

    [SerializeField] float mouseSense = 70f;
    public Transform playerBody;

    //clamp onto an object you're holding
    public bool holding;
    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSense * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSense * Time.deltaTime;

        xRotation -= mouseY;
        if (!holding)
            xRotation = Mathf.Clamp(xRotation, -90, 90f);
        else if (holding)
            xRotation = Mathf.Clamp(xRotation, 0f, 0f); ;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

      
    }
}
