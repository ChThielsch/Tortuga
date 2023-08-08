using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CloudFine.FlockBox;

public class FlockMesh : MonoBehaviour
{
    public FlockBox[,] mesh;

    [Range(1, 20)] 
    public float 
        xMesh = 1,
        yMesh = 1;

    [Range(10, 100)]
    public float
        xBox,
        yBox,
        zBox;

    public void RefreshMesh()
    {
        for (int x = 0; x < xMesh; x++)
            for (int y = 0; y < xMesh; y++)
            {

            }
    }

    public FlockBox CreateBox(Vector2 coords)
    {
        return null;
    }

    public Vector2 CoordsToPosition(Vector2 coords)
    {
        return new Vector2(coords.x * xBox, coords.y * yBox);
    }
    public Vector2 PositionToCoords(Vector3 pos)
    {
        Vector3 localPos = transform.worldToLocalMatrix * pos;
        Vector2 planePos = new Vector2(Mathf.Floor(localPos.x / xBox), Mathf.Floor(localPos.z / zBox));
        planePos = new Vector2(Mathf.Clamp(planePos.x, 0, xMesh), Mathf.Clamp(planePos.y, 0, yMesh));

        return Vector2.zero;
    }
}
