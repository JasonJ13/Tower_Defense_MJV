using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected Map map;

    [SerializeField]
    protected int range;

    protected Map.coords coord;

    protected Transform transform;

    protected virtual void OnEnable()
    {
        transform = GetComponent<Transform>();

        coord = Map.Instance.PositionToCoords(transform.position);
    }

    protected List<Map.coords> getNeighbours()
    {
        List<Map.coords> neighbours = new List<Map.coords>();

        for (int i = -range; i < 1 + range; i++)
        {
            for (int j = -range; j < range + 1; j++)
            {
                Map.coords tile = new Map.coords(coord.row + i, coord.column + j);

                if ((j != 0 || i != 0) && Map.Instance.IsInMap(tile))
                {
                    neighbours.Add(tile);
                }
            }
        }

        return neighbours;
    }
}
