using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RobotEnemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float randomPath;
    [SerializeField] private bool safeRoute;


    private Map.coords coord;
    private Map.coords previousCoord;

    private Vector3 destination;
    private int currentHP;

    private Animator anim;

    private List<Map.coords> destinations;

    private CharacterController characterController;
    private EnemyGraph enemyGraph;

    private Vector3 direction;
    private float angle;
    private bool isRandom=false;

    private bool marching = false;

    private IEnumerator Start()
    {
       
        anim = gameObject.GetComponent<Animator>();
        characterController = gameObject.GetComponent<CharacterController>();
        enemyGraph = gameObject.GetComponent<EnemyGraph>();


        currentHP = maxHP * (int)RobotFactory.Instance.GetHPMultiplier();


        float randChance = Random.Range(0f, 1f);
        if (randChance < randomPath)
        {
            isRandom = true;
            Debug.Log("RANDOM");

        }
        while (enemyGraph.GetGraph()==null)
        {
            yield return null;
        }

        
        if (!isRandom)
        {
            this.destinations = enemyGraph.GetPathFinding(coord);
            
        }
        ChangeDestination();
        
    }
    public void SetStart(Map.coords start)
    {
        this.coord= start;
    }

    private void Update()
    {
        if (destination != Vector3.zero && marching)
        {
            //Debug.Log(destination);
            Vector3 rot = new Vector3(0, angle, 0);
            transform.eulerAngles = Vector3.Lerp(
                transform.rotation.eulerAngles,
                rot,
                Time.deltaTime
            );

            direction = destination - transform.position;

            direction.y = 0;
            var distance = direction.magnitude;
            direction = direction / distance;

            if (distance < 0.05)
            {
                if (isRandom)
                {
                    coord = Map.Instance.PositionToCoords(transform.position);
                    if (enemyGraph.IsExit(coord))
                    {
                        Attack();
                    }
                    else
                    {
                        ChangeDestination();
                    }
                }
                else
                {
                    if (destinations.Count > 0)
                    {
                        Debug.Log("change destination");
                        ChangeDestination();
                    }
                    else
                    {
                        Attack();

                    }
                }
            }
            else
            {
                characterController.Move(direction * speed * Time.deltaTime);
            }
        }
    }
    private void Attack()
    {
        marching = false;
        Player.Instance.Damage(damage);
        anim.SetTrigger("Attack");

    }
    private void ChangeDestination()
    {
        Debug.Log("inchangedestination");
        if (this.isRandom)
        {
            previousCoord = coord;
            coord = Map.Instance.PositionToCoords(transform.position);
            var new_coords = enemyGraph.GetRandomNeighboor(coord,previousCoord);
            this.destination = Map.Instance.CoordsToPosition(new_coords);

        }
        else
        {
            var new_coords = destinations.FirstOrDefault();
            destinations.Remove(new_coords);

            this.destination = Map.Instance.CoordsToPosition(new_coords);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("Die");
    }

    protected void DeathAnimFinished() //event de fin de l'anim de mort
    {

        RobotFactory.Instance.DestroyRobot(this.transform.parent.gameObject);

    }

    protected void FinishedOpening() //event de fin de l'animation opening
    {
        marching = true;
    }
}
