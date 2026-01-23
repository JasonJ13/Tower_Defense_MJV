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

    private List<Map.coords> TileRoad;
    private float timer;

    private int supplied = 0;

    private void OnEnable()
    {
        //construct_road();
    }

    private void check_type(Map.coords tile)
    {
        Map.TileType type = Map.Instance.GetMapArrayCoords(tile);

        switch (type)
        {
            case Map.TileType.road:
            case Map.TileType.cross:
                TileRoad.Add(tile);
                break;

            case Map.TileType.start:
                TileRoad.Insert(0, tile);
                break;

            case Map.TileType.end:
                TileRoad.Add(tile);
                break;

            case Map.TileType.construct:
                //check si la construction est un générateur
                break;
        }
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
