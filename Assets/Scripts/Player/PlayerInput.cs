using Assets.Scripts.EventBus;
using Assets.Scripts.Events;
using Assets.Scripts.Units;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Rigidbody cameraTarget;
    [SerializeField] Camera mainCamera;
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] RectTransform selectionBox;
    [SerializeField] LayerMask clickableLayerMask;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float edgePanSize = 50f;
    [SerializeField] float panSpeed = 15f;
    [SerializeField] float zoomSpeed = 5;
    [SerializeField] float rotationSpeed = 2.5f;
    [SerializeField] float minZoomDistance = 7.5f;

    CinemachineFollow cinemachineFollow;
    Vector3 startingFollowOffset;
    Vector2 startingMousePosition;
    float zoomStartTime;
    float rotationStartTime;
    float maxRotationDistance;

    HashSet<AbstractUnit> aliveUnits = new(100);
    HashSet<AbstractUnit> selectionBoxUnits = new(20);
    List<ISelectable> selectedUnits = new(20);

    void Awake()
    {
        if (!cinemachineCamera.TryGetComponent(out cinemachineFollow))
        {
            Debug.LogError("Cinemachine Camera does not have a CinemachineFollow component.");
        }

        startingFollowOffset = cinemachineFollow.FollowOffset;
        maxRotationDistance = Mathf.Abs(startingFollowOffset.z);

        Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
        Bus<UnitSelectedEvent>.OnEvent += HandleUnitSelection;
        Bus<UnitDeselectedEvent>.OnEvent += HandleUnitDeselection;
    }

    // Update is called once per frame
    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleRotating();
        HandleRightClick();
        HandleDrag();
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

    void HandleLeftClick()
    {
        if (!mainCamera) return;

        Ray cameraRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, clickableLayerMask) && hit.collider.TryGetComponent(out ISelectable selectable))
        {
            selectable.Select();
        }
    }

    void HandleRightClick()
    {
        if (selectedUnits.Count == 0) return;

        Ray cameraRay = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Mouse.current.rightButton.wasReleasedThisFrame && Physics.Raycast(cameraRay, out RaycastHit hit, float.MaxValue, groundLayerMask))
        {
            List<AbstractUnit> abstractUnits = new(selectedUnits.Count);
            foreach (ISelectable selectable in selectedUnits)
            {
                if (selectable is AbstractUnit unit)
                {
                    abstractUnits.Add(unit);
                }
            }

            int unitsOnLayer = 0;
            int maxUnitsPerLayer = 1;
            float circleRadius = 0f;
            float radialoffset = 0f;

            foreach (AbstractUnit unit in abstractUnits)
            {
                Vector3 targetPosition = new(
                    hit.point.x + circleRadius + Mathf.Cos(radialoffset * unitsOnLayer),
                    hit.point.y,
                    hit.point.z + circleRadius + Mathf.Sin(radialoffset * unitsOnLayer)
                );

                unit.MoveTo(targetPosition);
                unitsOnLayer++;

                if (unitsOnLayer >= maxUnitsPerLayer)
                {
                    unitsOnLayer = 0;
                    circleRadius += unit.agentRadius * 2f;
                    maxUnitsPerLayer = Mathf.FloorToInt(2 * Mathf.PI * circleRadius / (unit.agentRadius * 2f));
                    radialoffset = 2 * Mathf.PI / maxUnitsPerLayer;
                }
            }
        }
    }

    void HandleDrag()
    {
        if (!selectionBox) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            selectionBox.sizeDelta = Vector2.zero;
            startingMousePosition = Mouse.current.position.ReadValue();
            selectionBox.gameObject.SetActive(true);
            selectionBoxUnits.Clear();
        }
        else if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame)
        {
            Bounds selectionBoxBounds = ResizeSelectionBox();

            foreach (AbstractUnit unit in aliveUnits)
            {
                Vector2 unitPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (selectionBoxBounds.Contains(unitPosition))
                {
                    selectionBoxUnits.Add(unit);
                }
            }
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (!Keyboard.current.shiftKey.isPressed) DeselectAll();

            HandleLeftClick();

            foreach (AbstractUnit unit in selectionBoxUnits)
            {
                unit.Select();
            }

            selectionBox.gameObject.SetActive(false);
            selectionBox.sizeDelta = Vector2.zero;
        }
    }

    Bounds ResizeSelectionBox()
    {
        Vector2 currentMousePosition = Mouse.current.position.ReadValue();
        float width = currentMousePosition.x - startingMousePosition.x;
        float height = currentMousePosition.y - startingMousePosition.y;
        selectionBox.anchoredPosition = startingMousePosition + new Vector2(width / 2, height / 2);
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

        return new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
    }

    void DeselectAll()
    {
        ISelectable[] currentlySelectedUnits = selectedUnits.ToArray();
        foreach (ISelectable unit in currentlySelectedUnits)
        {
            unit.Deselect();
        }
    }

    void HandleUnitSpawn(UnitSpawnEvent evt)
    {
        aliveUnits.Add(evt.Unit);
    }

    void HandleUnitSelection(UnitSelectedEvent evt)
    {
        selectedUnits.Add(evt.Unit);
    }

    void HandleUnitDeselection(UnitDeselectedEvent evt)
    {
        selectedUnits.Remove(evt.Unit);
    }

    private void OnDestroy()
    {
        Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
        Bus<UnitSelectedEvent>.OnEvent -= HandleUnitSelection;
        Bus<UnitDeselectedEvent>.OnEvent -= HandleUnitDeselection;
    }
}
