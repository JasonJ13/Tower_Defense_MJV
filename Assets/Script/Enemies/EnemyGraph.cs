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

    public Map.coords GetRandomNeighboor(Map.coords start, Map.coords previousCoord)
    {
        var neighboors = graph.GetNeighboors(start);
        neighboors.Remove(previousCoord);
        int i = Random.Range(0, neighboors.Count);
        return neighboors[i];
    }

    public bool IsExit(Map.coords coord)
    {
        return (graph.GetNodeInfos(coord).type == Map.TileType.end);
    }


    public List<Map.coords> GetPathFinding(Map.coords start, bool towerWeight)
    {
        if (!towerWeight)
        {
            var list = graph.GetPath(start, graph.GetNearestEnd(start));
            list.Remove(start);
            return list;
        }
        else
        {
            var dictTowers = Player.Instance.GetDictCoordsTower();
            foreach(Map.coords coord in dictTowers.Keys)
            {
                Player.TowerType tower = dictTowers[coord];
                int range=0;
                int weight=0;
                switch(tower)
                {
                    case Player.TowerType.Cannon:
                        range = 2;
                        weight = 2;
                        break;
                    case Player.TowerType.Archer:
                        range = 3;
                        weight = 1;
                        break;
                    case Player.TowerType.Turret:
                        range = 2;
                        weight = 1;
                        break;
                    case Player.TowerType.Mage:
                        range = 1;
                        weight = 2;
                        break;
                }
                int row = coord.row;
                int column = coord.column;
                for (int i = 0; i<range; i++)
                {
                    for (int j = 0; j<range; j++)
                    {
                        Map.coords possible_path = new Map.coords(row+i, column+j);
                        if (graph.GetNodeInfos(possible_path).type == Map.TileType.road)
                        {
                            var edge = Map.Instance.FindEdge(possible_path);
                            graph.AddWeight(edge, weight);
                        }
                        

                    }
                }
            }
            graph.UpdateGraph();
            var list = graph.GetPath(start, graph.GetNearestEnd(start));
            list.Remove(start);
            return list;
        }
       

    }

    


    

    
    
}
