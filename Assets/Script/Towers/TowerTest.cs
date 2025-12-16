using UnityEngine;

public class TowerTest : Tower
{

    private float timer = 3f;

    private void Start()
    {
        nameTower = "Tower Test";
        tower = get_tower();
        Debug.Log(get_head().position);

    }

    protected override void shoot()
    {
        Debug.Log("shoot");
        Vector3 PositionBullet = new Vector3(get_head().position.x + 3,get_head().position.y,get_head().position.z);
        Instantiate(bullet, PositionBullet, Quaternion.identity);
    }

    private void Update()
    {
        if (timer > 0f)
        {
          timer -= Time.deltaTime;  
        } else
        {
            shoot();
            timer = 3f;
        }
        


    }
    
}
