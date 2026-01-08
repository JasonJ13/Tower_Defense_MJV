using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
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
    protected Map.coords tile;
    private List<Map.coords> TileRoad;

    protected void initTowerParameter(int d, int r, string name)
    {
        dmg = d;
        range = r;
        nameTower = name;
        tile = Map.Instance.PositionToCoords(GetComponent<Transform>().position);
    }

    private void OnEnable()
    {
        construct_road();
    }

    private void check_type(Map.TileType type)
    {
        switch (type)
        {
            case Map.TileType.road:
            case Map.TileType.cross:
                //ajouter à la liste
                break;

            case Map.TileType.start:
                //ajouter à la fin de la liste
                break;

            case Map.TileType.end:
                //ajouter au début
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
                // Vérification des cases
            }
        }
    }

    protected abstract void shoot();

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
