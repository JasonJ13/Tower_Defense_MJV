using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotEnemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;

    //[SerializeField] private Vector2[] destinations;

    private Vector2 destination;
    private int currentHP;

    private Animator anim;

    //valeurs provisoires
    //private float redModifier = 0.5f;
    //private float blueModifier = 1f;
    //private float greenModifier = 2f;
    //private float yellowModifier = 1f;


    
    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();

        currentHP = maxHP;
        
    }
    

    private void Update()
    {
        

    }

    private void SetDestination(Vector2 destination)
    {
        this.destination = destination;
    }



    public void TakeDamage(int damage)
    {
        //il faut multiplier les dégats par le modifier selon le type de la tour, à voir
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    
    
    public void Die()
    {
        anim.SetBool("Open_Anim", false);

    }
    public void DeathAnimFinished()
    {
        Debug.Log("mort");
        Destroy(gameObject,0.5f);
    }


}
