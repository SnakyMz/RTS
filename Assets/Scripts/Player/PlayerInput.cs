using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] float keyboardPanSpeed = 15f;
    [SerializeField] float zoomSpeed = 5;
    [SerializeField] float minZoomDistance = 7.5f;

    CinemachineFollow cinemachineFollow;
    Vector3 startingFollowOffset;
    float zoomStartTime;

    void Awake()
    {
        if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
        {
            Debug.LogError("Cinemachine Camera does not have a CinemachineFollow component.");
        }

        startingFollowOffset = cinemachineFollow.FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePanning();
        HandleZooming();
    }

    private void HandlePanning()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
        {
            moveInput.y += keyboardPanSpeed;
        }
        if (Keyboard.current.downArrowKey.isPressed)
        {
            moveInput.y -= keyboardPanSpeed;
        }
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            moveInput.x -= keyboardPanSpeed;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            moveInput.x += keyboardPanSpeed;
        }

        moveInput *= Time.deltaTime;
        cameraTarget.position += new Vector3(moveInput.x, 0, moveInput.y);
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
}
