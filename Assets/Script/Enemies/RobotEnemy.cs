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
    private Map.coords coord;

    private Animator anim;

    private List<Map.coords> destinations; 

    private CharacterController characterController;

    private Vector3 direction;
    private float angle;

    private bool marching = false;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        currentHP = maxHP;
        characterController = gameObject.GetComponent<CharacterController>();

        this.destinations = new List<Map.coords>() { new Map.coords(1, 5), new Map.coords(4,5) }; //valeurs temporaires
        this.coord = new Map.coords(1, 1);
        transform.position = Map.Instance.CoordsToPosition(this.coord); //valeur temporaire

        

        if (destinations.Count > 0 )
            ChangeDestination();


        

    }

    private void Update()
    {
        if (destination !=null && marching)
        {

            Vector3 rot = new Vector3(0, angle, 0);
            transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, rot, Time.deltaTime);


            direction = destination - transform.position;

            direction.y = 0;
            var distance = direction.magnitude;
            direction = direction / distance;



            if (distance<0.05)
            {
                transform.position = destination; //petit tp pour éviter que le robot parte dans tous les sens

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
                //characterController.Move(direction * speed * Time.deltaTime);
            }
        }
        

    }




    private void ChangeDestination()
    {
        var new_coords = destinations.FirstOrDefault();
        destinations.Remove(new_coords);


        this.destination = Map.Instance.CoordsToPosition(new_coords);
        
    
        angle = 90;
        Debug.Log(angle);
        


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
