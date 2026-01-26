using System.Collections.Generic;
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

    public int level;

    public int waveIndex;

    private List<RobotEnemy> robots = new List<RobotEnemy>();

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
}
