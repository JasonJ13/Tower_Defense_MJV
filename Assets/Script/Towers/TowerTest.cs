using UnityEngine;

public class TowerTest : Tower
{

    private float timer = 3f;

    private void Start()
    {
        nameTower = "Tower Test";
        towerTransform = get_tower_transform();

    }

    protected override void shoot()
    {
        Debug.Log("shoot");
        Vector3 PositionBullet = new Vector3(towerTransform.position.x + 3, towerTransform.position.y, towerTransform.position.z);
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
