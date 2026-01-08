using System.Runtime.CompilerServices;
using UnityEngine;


public abstract class Tower : MonoBehaviour
{

    [SerializeField] protected int range;
    [SerializeField] protected int dmg;
    [SerializeField] protected GameObject bullet;
    private int supplied = 0;

    protected Transform towerTransform;
    protected string nameTower;

    private int[] TileRoad;



    public bool is_connected()
    {
        return supplied > 0;
    }

    public void connected() 
    {
        supplied ++ ;
    }

    public void disconnected() 
    {
        supplied -- ;
    }



    protected void initTowerParameter (int d, int r)
    {
        dmg = d;
        range = r;
    }

    public void construct_road ()
    {

    }



    protected abstract void shoot();

}
