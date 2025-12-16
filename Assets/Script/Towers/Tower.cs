using System.Runtime.CompilerServices;
using UnityEngine;


public abstract class Tower : MonoBehaviour
{

    [SerializeField] protected int range;
    [SerializeField] protected int dmg;
    [SerializeField] protected GameObject bullet;

    protected GameObject tower;
    protected string nameTower;


    public void initTower (int d, int r)
    {
        dmg = d;
        range = r;
    } 

    protected abstract void shoot();



    protected GameObject get_tower()
    {
        return GameObject.Find(nameTower);
    }
    

    protected Transform get_head() 
    {
        return tower.transform.Find("Head");
    }
}
