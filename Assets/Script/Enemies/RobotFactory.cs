using NUnit.Framework;
using System.Collections.Generic;
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

    public int level;

    public int waveIndex;

    private Map.Graph graph;
    private List<Map.coords> starts;
    private List<RobotEnemy> robots;

    private void Start()
    {
        graph = Map.Instance.GetGraph();
        starts = graph.GetAllStart();
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

    private void SpawnRobot(GameObject robot)
    {
        int rIndex = Random.Range(0, starts.Count);
        var start = this.starts[rIndex];

        GameObject enemy = Instantiate(robot, Map.Instance.CoordsToPosition(start), Quaternion.identity);
        robots.Add(enemy.GetComponent<RobotEnemy>());
    }

    
}
