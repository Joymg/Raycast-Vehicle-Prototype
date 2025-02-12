using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    public static int ROTATION_ANGLE = 90;
    [SerializeField] private List<GameObject> placedGameObjects = new();

    public int PlaceObject(RoadData data, Vector3 position, int turns)
    {
        GameObject newRoad = Instantiate(data.Prefab);
        newRoad.transform.position = position;
        newRoad.transform.Rotate(Vector3.up, ROTATION_ANGLE * turns);
        placedGameObjects.Add(newRoad);
        return placedGameObjects.Count - 1;
    }
}