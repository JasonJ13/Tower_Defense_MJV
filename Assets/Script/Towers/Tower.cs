using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected Map map;

    [SerializeField]
    protected int range;

    [SerializeField]
    protected string nameTower;

    protected Map.coords selfTile;

    protected Transform transform;

    protected virtual void initTowerParameter(string name)
    {
        transform = GetComponent<Transform>();

        selfTile = Map.Instance.PositionToCoords(transform.position);
    }

    private List<Map.coords> getNeighbours()
    {
        List<Map.coords> neighbours = new List<Map.coords>();

        for (int i = 0; i < 1 + range; i++)
        {
            for (int j = 0; j < range - i; j++)
            {
                Map.coords tile = new Map.coords(i, j);
                if ((j != 0 && i != 0) && Map.Instance.IsInMap(tile))
                {
                    neighbours.Add(tile);
                }
            }
        }

        return neighbours;
    }
}
