using UnityEngine;

public class Player : MonoBehaviour
{
    private int hp;

    private int money;

    public int GetHp()
    {
        return this.hp;
    }

    public int Damage(int damage)
    {
        this.hp = this.hp - damage;
        return this.hp;
    }

    public int GetMoney()
    {
        return this.money;
    }

    public int GainMoney(int moneyToAdd)
    {
        this.money = this.money + moneyToAdd;
        return this.money;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
