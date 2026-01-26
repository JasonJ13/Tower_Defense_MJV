using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OffensifTower : Tower
{
    [SerializeField]
    protected int dmg;

    [SerializeField]
    protected float shootRate = 60f;

    private Map.Graph graph;

    private List<Map.Graph.nodeInfos> path = new List<Map.Graph.nodeInfos>();
    private float timer;

    private int supplied = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        constructPath();
    }

    private void constructPath()
    {
        graph = Map.Instance.GetGraph();
        getNeighbours().ForEach(addPath);

        foreach (Map.Graph.nodeInfos node in path)
        {
            Debug.Log(node.distanceFromEnd);
        }
    }

    private void addPath(Map.coords tile)
    {
        Map.TileType type = Map.Instance.GetMapArrayCoords(tile);

        switch (type)
        {
            case Map.TileType.road:
            case Map.TileType.cross:
            case Map.TileType.start:
            case Map.TileType.end:
                path.Add(graph.GetNodeInfos(tile));
                break;

            case Map.TileType.generator:
                connected();
                break;
        }
        path.Sort();
    }

    public bool is_connected()
    {
        return supplied > 0;
    }

    public void connected()
    {
        supplied++;
    }

    public void disconnected()
    {
        supplied--;
    }

    private void Update()
    {
        if (timer > shootRate)
        {
            /*foreach (Map.coords chemin in TileRoad)
            {
                //Check si un ennemi est présent
                break;
            }*/
        }
        else
        {
            timer += 1f;
        }
    }
}
