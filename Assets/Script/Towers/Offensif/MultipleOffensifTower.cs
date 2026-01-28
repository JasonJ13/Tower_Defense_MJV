using UnityEngine;

public class MultipleOffensifTower : OffensifTower
{
    [SerializeField]
    private GameObject cloud;

    protected override void Shoot()
    {
        bool didntAttack = true;

        foreach (Map.coords tile in path)
        {
            if (RobotFactory.Instance != null)
            {
                RobotEnemy robotTarget = RobotFactory.Instance.FindRobotOnTile(tile);
                if (robotTarget != null)
                {
                    if (didntAttack)
                    {
                        didntAttack = false;
                        Instantiate(
                            cloud,
                            transformTower.position + new Vector3(0, 0.2f, 0),
                            transformTower.rotation
                        );
                    }

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
