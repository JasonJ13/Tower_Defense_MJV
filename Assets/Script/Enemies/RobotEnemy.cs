using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using Debug = UnityEngine.Debug;

public class RobotEnemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float randomPath;

    private Vector3 destination;
    private int currentHP;

    private Animator anim;

    private List<Map.coords> destinations; 

    private CharacterController characterController;

    private Vector3 direction;
    private float rotation;

    private bool marching = false;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        currentHP = maxHP;
        characterController = gameObject.GetComponent<CharacterController>();

        this.destinations = new List<Map.coords>() { new Map.coords(1, 5), new Map.coords(4,5) };
        Debug.Log(destinations.Count);
        if (destinations.Count > 0 )
            ChangeDestination();

        transform.position = Map.Instance.CoordsToPosition(new Map.coords(1, 1));
    }

    private void Update()
    {
        if (destination !=null && marching)
        {

            var pos = transform.position;
            
            direction = destination - transform.position;
            direction.y = 0;
            var distance = direction.magnitude;
            direction = direction / distance;
            Debug.Log(direction);
            Debug.Log(distance);

           
            if (distance<0.05)
            {
                transform.position = destination;
                if (destinations.Count > 0)
                {
                    Debug.Log("change destination");
                    ChangeDestination();
                }
                else
                {
                    anim.SetTrigger("Attack");
                    Debug.Log("Fin du parcours");
                }
                
            }
            else
            {
                characterController.Move(direction * speed * Time.deltaTime);
            }
        }
        

    }




    private void ChangeDestination()
    {
        var new_coords = destinations.FirstOrDefault();
        destinations.Remove(new_coords);
        this.destination = Map.Instance.CoordsToPosition(new_coords);
        Debug.Log(destination);
        this.rotation = Vector2.Angle(new Vector2(transform.position.x, transform.position.z), new Vector2(destination.x, destination.z));
        //Debug.Log(rotation);
        


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
    }

    protected void FinishedOpening() //event de fin de l'animation opening
    {
        marching = true;
    }


}
