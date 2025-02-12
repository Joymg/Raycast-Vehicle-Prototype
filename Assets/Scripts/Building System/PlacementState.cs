using System;
using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int ID;
    private int turns;
    private Grid grid;
    private PreviewSystem previewSystem;
    private RoadDatabase_SO database;
    private GridData buildingData;
    private ObjectPlacer buildingPlacer;

    public PlacementState(int id, Grid grid, PreviewSystem previewSystem,
        RoadDatabase_SO database, GridData buildingData, ObjectPlacer buildingPlacer)
    {
        ID = id;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.buildingData = buildingData;
        this.buildingPlacer = buildingPlacer;

        selectedObjectIndex = database.roadData.FindIndex(road => road.ID == ID);
        if (selectedObjectIndex > -1)
        {
            RoadData data = database.roadData[selectedObjectIndex];
            previewSystem.StartPreview(data.Prefab, data.Size);
        }
        else
        {
            throw new Exception($"No Object with ID {ID}");
        }
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!CheckPlacementIsValid(gridPosition, selectedObjectIndex))
            return;

        RoadData data = database.roadData[selectedObjectIndex];
        Vector3 worldPosition = gridPosition * (int)grid.cellSize.x;

        int index = buildingPlacer.PlaceObject(data, worldPosition + new Vector3Int(data.Size.x, 0, data.Size.y),
            turns);

        buildingData.AddObjectAt(gridPosition, data.Size, data.ID, index);
        previewSystem.MovePreview(worldPosition + new Vector3Int(data.Size.x, 0, data.Size.y));
        previewSystem.MoveCursor(worldPosition + CalculateCursorPositionOffset(data.Size));
        previewSystem.ApplyFeedback(false);
    }


    public void UpdateState(Vector3Int gridPosition, int turns)
    {
        this.turns = turns;
        bool isPlacementValid = CheckPlacementIsValid(gridPosition, selectedObjectIndex);

        RoadData data = database.roadData[selectedObjectIndex];
        previewSystem.RotatePreview(turns);
        Vector3Int worldPosition = gridPosition * (int)grid.cellSize.x;
        previewSystem.MovePreview(worldPosition + new Vector3Int(data.Size.x, 0, data.Size.y));
        previewSystem.MoveCursor(worldPosition + CalculateCursorPositionOffset(data.Size));
        previewSystem.ApplyFeedback(isPlacementValid);
    }

    private Vector3 CalculateCursorPositionOffset(Vector2Int dataSize)
    {
        Vector3 newCenter = new Vector3(grid.GetLayoutCellCenter().x * dataSize.x, 0,
            grid.GetLayoutCellCenter().y * dataSize.y);
        return newCenter;
    }

    public void EndState()
    {
        previewSystem.StopPreview();
    }

    private bool CheckPlacementIsValid(Vector3Int gridPosition, int index)
    {
        return buildingData.CanPlaceObjectAt(gridPosition, database.roadData[index].Size);
    }
}