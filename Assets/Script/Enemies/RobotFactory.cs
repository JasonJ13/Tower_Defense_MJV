using System.Collections;
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

    [SerializeField]
    private GameObject standardRobot;

    [SerializeField]
    private GameObject quickRobot;

    [SerializeField]
    private GameObject bigRobot;

    [SerializeField]
    private GameObject jesterRobot;

    private int level = 0;
    private int numbOfWaves;

    private int waveIndex = 0;
    private int monstersToSpawn = 2;
    private int monstersSpawned;

    [SerializeField] private float quickToBigRatio;

    private Map.Graph graph;
    private List<Map.coords> starts;
    private List<RobotEnemy> robots = new List<RobotEnemy>();

    private bool ready = false;


    private IEnumerator Start()
    {
        while (Map.Instance.GetGraph() == null) //attend le calcul du graphe
        {
            yield return null;
        }

        graph = Map.Instance.GetGraph();
        starts = graph.GetAllStart();

        SetUpLevel();
        SetUpWave();
        ready = true;
    }

    private void Update()
    {
        if (monstersSpawned < monstersToSpawn && ready)
        {
            float monsterProb = Random.Range(0f, 2f);
            if (monsterProb > 1f)
            {
                StartCoroutine(SpawnRobot(standardRobot));
            }
            else if (monsterProb < 0.15f)
            {
                StartCoroutine(SpawnRobot(jesterRobot));

            }
            else if (monsterProb < quickToBigRatio)
            {
                StartCoroutine(SpawnRobot(quickRobot));
            }
            else
            {
                StartCoroutine(SpawnRobot(bigRobot));
            }
        }

        if (monstersSpawned==monstersToSpawn && robots.Count==0 && ready)
        {
            Debug.Log("wave finished");
            if (waveIndex==numbOfWaves)
            {
                Debug.Log("next level");
                SetUpLevel();
            }
            SetUpWave();
        } 

    }

    public float GetHPMultiplier()
    {
        if (level > 1)
        {
            float levelMultiplier = level + level / 10;
            return levelMultiplier;
        }

        else
            return 1f;
        
    }

    private void SetUpLevel()
    {
        level++;
        numbOfWaves = level + Random.Range(0, 3);
        waveIndex = 0;
    }

    private void SetUpWave()
    {
        waveIndex++;
        monstersSpawned = 0;
        int difficultyLevel = level * waveIndex;
        monstersToSpawn += Random.Range(0, difficultyLevel);

    }

    public void DestroyRobot(GameObject robot)
    {
        robots.Remove(robot.GetComponentInChildren<RobotEnemy>());
        Destroy(robot);

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
        monstersSpawned++;

        int rIndex = Random.Range(0, starts.Count);
        var start = this.starts[rIndex];

        while (FindRobotOnTile(start) != null)          //si il y a un robot sur la tile, on attend que le robot parte
        {            
            yield return null;
        }

        GameObject enemy = Instantiate(robot, Map.Instance.CoordsToPosition(start), Quaternion.identity);
        RobotEnemy enemyScript = enemy.GetComponentInChildren<RobotEnemy>();
        enemyScript.SetStart(start);
        robots.Add(enemyScript);



    }
}
