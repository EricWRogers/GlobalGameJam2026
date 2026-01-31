using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int totalHealth;
    public int currentHealth;

    public UnityEvent damagedEvent;
    public UnityEvent healEvent;
    public UnityEvent outOfHealthEvent;
    public UnityEvent onReviveEvent;

    void Start()
    {
        if(damagedEvent == null)
            damagedEvent = new UnityEvent();
        if(healEvent == null)
            healEvent = new UnityEvent();
        if(outOfHealthEvent == null)
            outOfHealthEvent = new UnityEvent();
        if(onReviveEvent == null)
            onReviveEvent = new UnityEvent();
        currentHealth = totalHealth;
    }

    public void TakeDamage(int _dmg)
    {
        currentHealth -= _dmg;
        if(currentHealth <= 0)
        {
            outOfHealthEvent.Invoke();
        }
        else
        {
            damagedEvent.Invoke();
        }
    }
    public void Heal(int _healAmount)
    {
        currentHealth += _healAmount;
        if(currentHealth > totalHealth)
        {
            currentHealth = totalHealth;
        }
        healEvent.Invoke();
    }
    public void Revive()
    {
        currentHealth = totalHealth;
        onReviveEvent.Invoke();
    }

}
