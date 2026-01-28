using UnityEngine;

public class MultipleOffensifTower : OffensifTower
{
    protected override void Shoot()
    {
        foreach (Map.coords tile in path)
        {
            if (RobotFactory.Instance != null)
            {
                RobotEnemy robotTarget = RobotFactory.Instance.FindRobotOnTile(tile);
                if (robotTarget != null)
                {
                    robotTarget.TakeDamage(this.dmg);
                    timer = 0;
                }
            }
        }
    }

    private void Update()
    {
        if (CanShoot() && is_connected())
        {
            Shoot();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
