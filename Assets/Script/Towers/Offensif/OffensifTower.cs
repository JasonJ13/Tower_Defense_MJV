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

    protected List<Map.coords> path = new List<Map.coords>();
    protected float timer;

    private int supplied = 0;

    protected virtual void OnEnable()
    {
        base.OnEnable();
        constructPath();
    }

    protected void constructPath()
    {
        getNeighbours().ForEach(addPath);
        getNeighbours(3).ForEach(SearchGenerator);
    }

    protected void addPath(Map.coords tile)
    {
        Map.TileType type = Map.Instance.GetMapArrayCoords(tile);

        switch (type)
        {
            case Map.TileType.road:
            case Map.TileType.cross:
            case Map.TileType.start:
                //Debug.Log(tile);
                path.Add(tile);
                break;
        }
        path.Sort(Map.CompareCoords);
    }

    protected void SearchGenerator(Map.coords tile)
    {
        Map.TileType type = Map.Instance.GetMapArrayCoords(tile);

        if (type == Map.TileType.generator)
        {
            connected();
        }
    }

    protected bool is_connected()
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

    protected bool CanShoot()
    {
        return timer > shootRate;
    }

    protected virtual void Shoot() { }

    private void dealDamage() { }
}
