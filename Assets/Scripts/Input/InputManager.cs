using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;

    private Vector3 _lastPosition;
    [SerializeField] private LayerMask placementLayerMask;

    public event Action OnClicked, OnExit;

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, placementLayerMask))
        {
            _lastPosition = hit.point;
        }

        return _lastPosition;
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
            OnClicked?.Invoke();

        if (Input.GetKeyDown(KeyCode.Escape)) 
            OnExit?.Invoke();
    }
}