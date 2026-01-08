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

    protected Transform towerTransform;
    protected string nameTower;
    protected int row;
    protected int colum;
    private List<Vector2> TileRoad;

    protected void initTowerParameter(int d, int r, string name)
    {
        dmg = d;
        range = r;
        nameTower = name;
        // tile = Map.gettile(transform.position.x, transform.position.y)
    }

    private void OnEnable()
    {
        construct_road();
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

    public void construct_road() { }

    protected abstract void shoot();
}
