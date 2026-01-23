using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotEnemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;
    [SerializeField] private int damage;
    [SerializeField] private float randomPath;

    private Vector2 destination;
    private int currentHP;

    private Animator anim;

    private EnemyGraph graph;


    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        currentHP = maxHP;

    }

    

    private void SetDestination(Vector2 destination)
    {
        this.destination = destination;
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
        anim.SetBool("Open_Anim", false);

    }
    protected void DeathAnimFinished()
    {
        
        Debug.Log("mort");
        Destroy(gameObject,0.5f);
    }


}
