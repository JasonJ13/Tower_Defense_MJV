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

    private List TileRoad


        public bool is_connected()
    {
        return supplied > 0
    }
    public void connected() 
    {
        supplied ++ ;
    }
    public void disconnected() 
    {
        supplied -- ;
    }

    public void initTower (int d, int r)
    {
        dmg = d;
        range = r;
    } 

    protected abstract void shoot();



    protected Transform get_tower_transform()
    {
        return GetComponent<Transform>();
    }
    


}
