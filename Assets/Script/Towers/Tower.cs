using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected Map map;

    [SerializeField]
    protected int range;

    protected Map.coords coords;

    protected Transform transformTower;

    protected virtual void OnEnable()
    {
        transformTower = GetComponent<Transform>();
        coords = Map.Instance.PositionToCoords(transformTower.position);
    }

    protected List<Map.coords> getNeighbours(int ran = 0)
    {
        List<Map.coords> neighbours = new List<Map.coords>();

        if (ran == 0)
        {
            ran = range;
        }

        for (int i = -ran; i < 1 + ran; i++)
        {
            for (int j = -ran; j < ran + 1; j++)
            {
                Map.coords tile = new Map.coords(coords.row + i, coords.column + j);

                if ((j != 0 || i != 0) && Map.Instance.IsInMap(tile))
                {
                    neighbours.Add(tile);
                }
            }
        }

        return neighbours;
    }
}
