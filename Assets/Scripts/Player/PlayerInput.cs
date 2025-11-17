using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float keyboardPanSpeed = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
}
