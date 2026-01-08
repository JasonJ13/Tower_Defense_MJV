using UnityEngine;

public class TowerTest : Tower
{

    private float timer = 3f;

    private void Start()
    {
        nameTower = "Tower Test";
    }

    protected override void shoot()
    {
        Debug.Log("shoot");
        Vector3 PositionBullet = new Vector3(GetComponent<Transform>().position.x + 3, GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
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
