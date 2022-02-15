using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMap
{
    private const int extent = 64;
    public const float voxelSize = 0.1f;
    public Vector3 origin = Vector3.zero;

    public float bounds {
        get { return ((float) extent) * voxelSize; }
    }

    // 128^3 bits
    // x, then y, then z
    public List<List<BitArray>> map;

    // super gross, i'm sorry
    public List<List<List<GameObject>>> objects;

    public VoxelMap()
    {
        map = new List<List<BitArray>>();
        for (int i = 0; i < extent; i++)
        {
            var middle = new List<BitArray>();
            for (int j = 0; j < extent; j++)
            {
                var inner = new BitArray(extent, false);
                middle.Add(inner);
            }
            map.Add(middle);
        }

        objects = new List<List<List<GameObject>>>();
        for (int k = 0; k < extent; k++)
        {
            var outer = new List<List<GameObject>>();
            for (int i = 0; i < extent; i++)
            {
                var middle = new List<GameObject>();
                for (int j = 0; j < extent; j++)
                {
                    middle.Add(null);
                }
                outer.Add(middle);
            }
            objects.Add(outer);
        }
    }

    /// <summary>
    /// Tells if the voxel is used by the painting system.
    /// </summary>
    /// <param name="point">The point that the user wishes to paint (in world space).</param>
    /// <returns>Whether or not the voxel has a paint splotch in it.</returns>
    public bool isVoxelUsed(Vector3 point)
    {
        // check if voxel is within the bounds
        Vector3 upperBound = origin + new Vector3(bounds, bounds, bounds);

        Debug.Log("Point: " + point);
        Debug.Log("Bounds: " + origin + " to " + upperBound);
        if (point.x >= upperBound.x || point.x < origin.x) return true;
        if (point.y >= upperBound.y || point.y < origin.x) return true;
        if (point.z >= upperBound.z || point.z < origin.x) return true;

        Vector3Int index = getVoxelIndex(point);
        return map[index.x][index.y][index.z];
    }

    /// <summary>
    /// Sets a voxel as used.
    /// </summary>
    /// <param name="point">The point that the user wishes to paint/unpaint (in world space).</param>
    /// <param name="used">Whether or not to set the voxel as used.</param>
    /// <returns>The index of the voxel that was set as used.</returns>
    public Vector3Int setVoxel(Vector3Int index, bool used)
    {
        map[index.x][index.y][index.z] = used;
        return index;
    }

    public Vector3Int getVoxelIndex(Vector3 point)
    {
        return new Vector3Int(
            (int)(point.x / voxelSize),
            (int)(point.y / voxelSize),
            (int)(point.z / voxelSize)
        );
    }
}
