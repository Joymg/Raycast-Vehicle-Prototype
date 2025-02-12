using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PreviewSystem previewSystem;
    [SerializeField] private ObjectPlacer buildingPlacer;
    [SerializeField] private Grid grid;

    [SerializeField] private RoadDatabase_SO roadDatabase;
    private int selectedRoadIndex = -1;

    [SerializeField] private GameObject gridVisualization;
    private GridData buildingData;

    private Vector3Int lastValidPosition = Vector3Int.zero;

    [SerializeField] private IBuildingState buildingState;
    private int _turns;
    private bool dirtyUpdate;

    private void Start()
    {
        StopPlacement();
        buildingData = new();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();

        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, previewSystem, roadDatabase, buildingData,
            buildingPlacer);
        dirtyUpdate = true;
        inputManager.OnClicked += PlaceRoad;
        inputManager.OnExit += StopPlacement;
    }


    private void PlaceRoad()
    {
        if (inputManager.IsPointerOverUI())
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        selectedRoadIndex = -1;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        buildingState = null;
        lastValidPosition = Vector3Int.zero;

        inputManager.OnClicked -= PlaceRoad;
        inputManager.OnExit -= StopPlacement;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(i, 0, j));
                Gizmos.DrawSphere(worldPosition, 0.5f);
            }
        }
    }

    private void Update()
    {
        HandleInput();

        dirtyUpdate |= HandleRotation();

        if (buildingState == null)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        //Debug.Log("gridPosition = " + gridPosition);

        if (lastValidPosition != gridPosition || dirtyUpdate)
        {
            buildingState.UpdateState(gridPosition, _turns);
            lastValidPosition = gridPosition;
        }
    }

    private bool HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _turns++;
            _turns %= 4;
            return true;
        }

        return false;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartPlacement(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartPlacement(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartPlacement(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartPlacement(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            StartPlacement(4);
    }
}