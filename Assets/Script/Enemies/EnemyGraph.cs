using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyGraph : MonoBehaviour
{
    [SerializeField] private bool safeRoute;

    private List<Map.coords> destinations;

    private Map.Graph graph;

    private IEnumerator Start()
    {
        while (Map.Instance.GetGraph() == null) //attend le calcul du graphe
        {
            yield return null;
        }

        graph = Map.Instance.GetGraph();
    }


    private void PathFindingDjikstra()
    {

    }
    
}
