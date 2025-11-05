using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _health = 100;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        //Debug.Log("Player health: " + _health);
    }
}
