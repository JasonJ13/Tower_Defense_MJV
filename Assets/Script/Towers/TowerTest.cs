using UnityEngine;

public class TowerTest : Tower
{
    //private float timer = 3f;

    private void Start()
    {
        initTowerParameter(3, 3, "Tower Test");
    }

    protected override void shoot() { }

    /*private void Update()
    {
        if (timer > 0f)
        {
          timer -= Time.deltaTime;
        } else
        {
            shoot();
            timer = 3f;
        }
        


    }*/
}
