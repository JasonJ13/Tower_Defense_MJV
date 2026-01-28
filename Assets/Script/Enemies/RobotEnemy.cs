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

    [SerializeField] private int rotationSpeed;

    //components
    private CharacterController characterController;
    private EnemyGraph enemyGraph;
    private Animator anim;

    private int currentHP;

    //path
    private List<Map.coords> destinations;
    private Vector3 destination;
    private Map.coords coord;
    private Map.coords previousCoord;


    private Vector3 direction;
    private float angle;
    

    private bool marching = false;
    private bool isRandom = false;

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
            this.destinations = enemyGraph.GetPathFinding(coord,safeRoute);
            
        }
        ChangeDestination();
        SetInitialAngle();

    }
    public void SetStart(Map.coords start)
    {
        this.coord= start;
    }

    private void Update()
    {
        if (destination != Vector3.zero && marching)
        {
            //maj rotation
            Vector3 rot = new Vector3(0, angle, 0);
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, rot, Time.deltaTime*rotationSpeed);

            //maj direction
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
                        ChangeRotation();
                    }
                }
                else
                {
                    if (destinations.Count > 0)
                    {
                        Debug.Log("change destination");
                        ChangeDestination();
                        ChangeRotation();
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

    private void SetInitialAngle()
    {
        
        var vectDirection = destination - transform.position;
        var angleToDest = Vector3.Angle(Vector3.forward, vectDirection);

        angle = angleToDest;
        transform.eulerAngles=new Vector3(0,angle,0);

    }

    private void ChangeRotation()
    {
        var newCoord = Map.Instance.PositionToCoords(destination);
        var diff_row = newCoord.row - coord.row;
        var diff_column = newCoord.column - coord.column;
        Debug.Log(diff_row);
        Debug.Log(diff_column);
        if (diff_row>0)
        {
            angle -= 90;
        } else if (diff_row<0)
        {
            angle += 90;
        } else if (diff_column>0)
        {
            angle -= 90;
        } else if (diff_column<0)
        {
            angle += 90;
        }
        Debug.Log(angle);
    }

   

    private void Attack()
    {
        marching = false;
        Player.Instance.Damage(damage);
        anim.SetTrigger("Attack");

    }
    private void ChangeDestination()
    {
        if (this.isRandom)
        {
            previousCoord = coord;
            coord = Map.Instance.PositionToCoords(transform.position);
            var new_coords = enemyGraph.GetRandomNeighboor(coord,previousCoord);
            this.destination = Map.Instance.CoordsToPosition(new_coords);

        }
        else
        {
            previousCoord = coord;
            coord = Map.Instance.PositionToCoords(transform.position);
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
