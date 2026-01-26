using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OffensifTower : Tower
{
    [SerializeField]
    protected int dmg;

    [SerializeField]
    protected float shootRate;

    private List<Map.coords> path = new List<Map.coords>();
    private float timer;

    private int supplied = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        constructPath();
    }

    private void constructPath()
    {
        getNeighbours().ForEach(addPath);
    }

    private void addPath(Map.coords tile)
    {
        Map.TileType type = Map.Instance.GetMapArrayCoords(tile);

        switch (type)
        {
            case Map.TileType.road:
            case Map.TileType.cross:
            case Map.TileType.start:
                path.Add(tile);
                break;

            case Map.TileType.generator:
                connected();
                break;
        }
        path.Sort(Map.CompareCoords);
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
            Shoot();
        }
        else
        {
            timer += 1f;
        }
    }

    protected virtual void Shoot()
    {
        foreach (Map.coords tile in path)
        {
            RobotEnemy robotTarget = RobotFactory.Instance.FindRobotOnTile(tile);
            if (robotTarget != null)
            {
                Debug.Log("shoot");
                robotTarget.TakeDamage(this.dmg);
                timer = 0;
                return;
            }
        }
    }
}
