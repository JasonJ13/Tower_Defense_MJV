using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyGraph : MonoBehaviour
{

    private Map.Graph graph;

    private IEnumerator Start()
    {
        while (Map.Instance.GetGraph() == null) //attend le calcul du graphe
        {
            yield return null;
        }
        graph = Map.Instance.GetGraph();
    }
    public Map.Graph GetGraph()
    {
        return graph;
    }

    public Map.coords GetRandomNeighboor(Map.coords start)
    {
        Debug.Log("in get random neighboor");
        var neighboors = graph.GetNeighboors(start);
        Debug.Log(neighboors.Count);
        int i = Random.Range(0, neighboors.Count);
        Debug.Log(neighboors[i].ToString());

        return neighboors[i];
    }

    public bool IsExit(Map.coords coord)
    {
        return (graph.GetNodeInfos(coord).type == Map.TileType.end);
    }


    public List<Map.coords> GetPathFinding(Map.coords start)
    {
        return graph.GetPath(start, graph.GetNearestEnd(start));

    }

    


    

    
    
}
