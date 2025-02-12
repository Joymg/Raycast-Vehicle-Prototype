using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    public Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2 objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (Vector3Int position in positionToOccupy)
        {
            if (!placedObjects.TryAdd(position, data))
                throw new Exception($"Dictionary already contains this cell in position {position}");
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2 objectSize)
    {
        List<Vector3Int> returnValue = new();
        for (int i = 0; i < objectSize.x; i++)
        {
            for (int j = 0; j < objectSize.y; j++)
            {
                returnValue.Add(gridPosition + new Vector3Int(i, 0 , j));
            }
        }
        return returnValue;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2 objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (Vector3Int position in positionToOccupy)
        {
            if (placedObjects.ContainsKey(position))
                return false;
        }

        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int id, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = id;
        PlacedObjectIndex = placedObjectIndex;
    }
    
}
