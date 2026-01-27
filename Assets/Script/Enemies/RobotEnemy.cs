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
        currentHP = maxHP;
        characterController = gameObject.GetComponent<CharacterController>();
        enemyGraph = gameObject.GetComponent<EnemyGraph>();

        float randChance = Random.Range(0f, 1f);
        //if (randChance < randomPath)
        //{
        //    isRandom=true;
            
        //}
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
        Debug.Log(coord);
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
        anim.SetTrigger("Attack");
        Debug.Log("Fin du parcours");
    }
    private void ChangeDestination()
    {
        Debug.Log("inchangedestination");
        if (this.isRandom)
        {
            Debug.Log("in if");
            //coord = Map.Instance.PositionToCoords(transform.position);
            var new_coords = enemyGraph.GetRandomNeighboor(coord);
            Debug.Log(new_coords);
            this.destination = Map.Instance.CoordsToPosition(new_coords);
            Debug.Log(destination);

        }
        else
        {
            var new_coords = destinations.FirstOrDefault();
            destinations.Remove(new_coords);

            this.destination = Map.Instance.CoordsToPosition(new_coords);
        }
        Debug.Log(destination);
        angle = 90;
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
        Debug.Log("mort");
        RobotFactory.Instance.DestroyRobot(this.transform.parent.gameObject);

    }

    protected void FinishedOpening() //event de fin de l'animation opening
    {
        marching = true;
    }
}
