using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    protected Map map;

    [SerializeField]
    protected int range;

    [SerializeField]
    protected int dmg;

    [SerializeField]
    protected GameObject bullet;
    private int supplied = 0;

    protected string nameTower;
    protected int row;
    protected int colum;
    protected Map.coords selfTile;
    private List<Map.coords> TileRoad;

    protected void initTowerParameter(int d, int r, string name)
    {
        dmg = d;
        range = r;
        nameTower = name;
        selfTile = Map.Instance.PositionToCoords(GetComponent<Transform>().position);
    }

    private void OnEnable()
    {
        map = GameObject.Find("Map").GetComponent<Map>();
        construct_road();
    }

    private void check_type(Map.coords tile)
    {
        Map.TileType type = map.GetMapArrayCoords(tile);

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

    private void construct_road()
    {
        //Check tout les tiles dans la range de la tour pour paramêtré le chemin à cibler et les supplieds

        for (int i = 0; i < this.range; i++)
        {
            for (int j = 0; j < this.range - i; j++)
            {
                Map.coords tile = new Map.coords(i, j);

                bool inMap = map.IsInMap(tile);

                if (inMap)
                {
                    check_type(tile);
                }
            }
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
}
