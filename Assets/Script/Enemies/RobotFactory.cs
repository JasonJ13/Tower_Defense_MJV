using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;



public class RobotFactory : MonoBehaviour
{
    public static RobotFactory Instance { get; private set; }

    private void Awake()
    {
        if (RobotFactory.Instance != null)
        {
            Debug.LogError("Error : Instance of RobotFactory already exists");
        }
        RobotFactory.Instance = this;
    }

    [SerializeField] private GameObject standardRobot;
    [SerializeField] private GameObject quickRobot;
    [SerializeField] private GameObject bigRobot;
    [SerializeField] private GameObject jesterRobot;

    private int level;

    private int waveIndex = 0;

    private Map.Graph graph;
    private List<Map.coords> starts;
    private List<RobotEnemy> robots = new List<RobotEnemy>();


    private IEnumerator Start()
    {
        while (Map.Instance.GetGraph() == null) //attend le calcul du graphe
        {
            yield return null;
        }

        graph = Map.Instance.GetGraph();
        starts = graph.GetAllStart();

    }

    private void Update()
    {
        
        if (waveIndex <1 && graph != null )
        {
            waveIndex++;
            StartCoroutine(SpawnRobot(bigRobot));
            
            
        }
        


    }

    public RobotEnemy FindRobotOnTile(Map.coords coord)
    {
        foreach (RobotEnemy robot in robots)
        {
            Map.coords robotCoord = Map.Instance.PositionToCoords(robot.transform.position);
            if (robotCoord.Equals(coord))
            {
                return robot;
            }
        }
        return null;
    }

    private IEnumerator SpawnRobot(GameObject robot)
    {
        int rIndex = Random.Range(0, starts.Count);
        var start = this.starts[rIndex];
        Debug.Log(FindRobotOnTile(start));

        while (FindRobotOnTile(start) != null)          //si il y a un robot sur la tile, on attend que le robot parte
        {
            Debug.Log("en yield");
            
            yield return null;
        }

        GameObject enemy = Instantiate(robot, Map.Instance.CoordsToPosition(start), Quaternion.identity);
        robots.Add(enemy.GetComponent<RobotEnemy>());


    }

    
}
