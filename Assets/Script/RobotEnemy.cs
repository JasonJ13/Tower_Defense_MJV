using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnemy : MonoBehaviour
{
    [SerializeField] private int maxHP;
    [SerializeField] private float speed;

    private int currentHP;

    private Vector3 rot = Vector3.zero;
    
    private Animator anim;

    private Vector2 destination;

    
    
    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        gameObject.transform.eulerAngles = rot;
        
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
        currentHP -= damage;
       
    }

   

    
}
