using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const float BaseGravity = -5f; // Used when player is grounded so they don't lose ground contact when going down a slope
    private const float Gravity = -9.81f;

    public Transform Player;
    public Transform Camera;
    public Transform GroundCheck;
    public CharacterController Controller;

    public float MouseSensitivity = 300f;
    public float MovementSpeed = 12f;
    public float JumpHeight = 3f;

    public float GroundDistance = 0.1f;
    public LayerMask GroudMask;
    public bool IsOnGround;

    public float VerticalVelocity;
    public float RotationX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Player position
        bool wasOnGround = IsOnGround;

        IsOnGround = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroudMask);

        // Looking
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        RotationX -= mouseY;
        RotationX = Mathf.Clamp(RotationX, -90f, 90f);

        Camera.localRotation = Quaternion.Euler(RotationX, 0f, 0f);
        Player.Rotate(Vector3.up * mouseX);

        // Movement
        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");

        if (!IsOnGround) VerticalVelocity += Gravity * Time.deltaTime;
        else VerticalVelocity = BaseGravity;

        Vector3 move = Player.transform.right * movementX + Player.transform.forward * movementZ + new Vector3(0f, VerticalVelocity, 0f);
        Controller.Move(move * MovementSpeed * Time.deltaTime);
    }
}
