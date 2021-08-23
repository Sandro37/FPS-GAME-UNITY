using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjHelth : MonoBehaviour
{
    [SerializeField] private int health;

    public void ApplyDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
