using UnityEngine;

public class SoloOffensialTower : OffensifTower
{
    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private float height;

    private Vector3 positionHead;

    protected Animator animationShoot;

    protected override void OnEnable()
    {
        base.OnEnable();

        animationShoot = GetComponent<Animator>();

        positionHead = transformTower.position;
        positionHead.y = height;
    }

    protected override void Shoot()
    {
        foreach (Map.coords tile in path)
        {
            if (RobotFactory.Instance != null)
            {
                RobotEnemy robotTarget = RobotFactory.Instance.FindRobotOnTile(tile);
                if (robotTarget != null)
                {
                    if (animationShoot != null)
                    {
                        animationShoot.SetTrigger("Shoot");
                    }

                    transformTower.LookAt(robotTarget.GetComponent<Transform>().position);
                    Instantiate(projectile, positionHead, transformTower.rotation);

                    robotTarget.TakeDamage(this.dmg);
                    timer = 0;
                    return;
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
