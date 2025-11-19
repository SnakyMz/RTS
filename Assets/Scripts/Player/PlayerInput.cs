using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Rigidbody cameraTarget;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float edgePanSize = 50f;
    [SerializeField] float panSpeed = 15f;
    [SerializeField] float zoomSpeed = 5;
    [SerializeField] float rotationSpeed = 2.5f;
    [SerializeField] float minZoomDistance = 7.5f;

    CinemachineFollow cinemachineFollow;
    Vector3 startingFollowOffset;
    float zoomStartTime;
    float rotationStartTime;
    float maxRotationDistance;

    void Awake()
    {
        if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
        {
            Debug.LogError("Cinemachine Camera does not have a CinemachineFollow component.");
        }

        startingFollowOffset = cinemachineFollow.FollowOffset;
        maxRotationDistance = Mathf.Abs(startingFollowOffset.z);
    }

    // Update is called once per frame
    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleRotating();
    }

    private void HandlePanning()
    {
        Vector2 moveInput = Vector2.zero;
        
        // Keyboard Input
        if (Keyboard.current.upArrowKey.isPressed)
        {
            moveInput.y += panSpeed;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            moveInput.y -= panSpeed;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput.x -= panSpeed;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput.x += panSpeed;
        }

        // Mouse Input
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        if (mousePosition.x <= edgePanSize)
        {
            moveInput.x -= panSpeed;
        }
        else if (mousePosition.x >= screenWidth - edgePanSize)
        {
            moveInput.x += panSpeed;
        }

        if (mousePosition.y <= edgePanSize)
        {
            moveInput.y -= panSpeed;
        }
        else if (mousePosition.y >= screenHeight - edgePanSize)
        {
            moveInput.y += panSpeed;
        }

        // Apply Movement
        cameraTarget.linearVelocity = new Vector3(moveInput.x, 0, moveInput.y);
    }

    void HandleZooming()
    {
        if (SetZoomTime())
        {
            zoomStartTime = Time.time;
        }

        float zoomTime = Mathf.Clamp01((Time.time - zoomStartTime) * zoomSpeed);
        Vector3 targetFollowOffset;

        if (Keyboard.current.endKey.isPressed)
        {
            targetFollowOffset = new Vector3(cinemachineFollow.FollowOffset.x, minZoomDistance, cinemachineFollow.FollowOffset.z);
        }
        else 
        {
            targetFollowOffset = new Vector3(cinemachineFollow.FollowOffset.x, startingFollowOffset.y, cinemachineFollow.FollowOffset.z);
        }

        cinemachineFollow.FollowOffset = Vector3.Slerp(cinemachineFollow.FollowOffset, targetFollowOffset, zoomTime);
    }

    bool SetZoomTime()
    {
        return Keyboard.current.endKey.wasPressedThisFrame || Keyboard.current.endKey.wasReleasedThisFrame;
    }

    void HandleRotating()
    {
        if (SetRotationTime())
        {
            rotationStartTime = Time.time;
        }
        float rotationTime = Mathf.Clamp01((Time.time - rotationStartTime) * rotationSpeed);
        Vector3 targetFollowOffset;

        if (Keyboard.current.pageDownKey.isPressed)
        {
            targetFollowOffset = new Vector3(maxRotationDistance, cinemachineFollow.FollowOffset.y, 0);
        }
        else if (Keyboard.current.pageUpKey.isPressed)
        {
            targetFollowOffset = new Vector3(-maxRotationDistance, cinemachineFollow.FollowOffset.y, 0);
        }
        else
        {
            targetFollowOffset = new Vector3(startingFollowOffset.x, cinemachineFollow.FollowOffset.y, startingFollowOffset.z);
        }

        cinemachineFollow.FollowOffset = Vector3.Slerp(cinemachineFollow.FollowOffset, targetFollowOffset, rotationTime);
    }

    bool SetRotationTime()
    {
        return Keyboard.current.pageDownKey.wasPressedThisFrame || Keyboard.current.pageUpKey.wasPressedThisFrame || Keyboard.current.pageDownKey.wasReleasedThisFrame || Keyboard.current.pageUpKey.wasReleasedThisFrame;
    }
}
