using UnityEngine;

// Author : 

public enum Direction
{
    Forward,
    Right,
    Back,
    Left
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction direction)
    {
        return (int)direction < 2 ? (direction + 2) : (direction - 2);
    }

    public static int Difference(this Direction direction, Direction otherDirection)
    {
        return ((int)direction - (int)otherDirection) % 4;
    }
    
    public static Direction Rotate(this Direction direction, int times)
    {
        return (Direction)(Mathf.Abs((int)direction + times) % 4);
    }

    public static Vector3 Collapse(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Forward:
                return Vector3.up;
            case Direction.Right:
                return Vector3.right;
            case Direction.Back:
                return Vector3.down;
            case Direction.Left:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }
}