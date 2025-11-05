using UnityEngine;

public class Wagon : MonoBehaviour
{
    [SerializeField] private int _health = 100;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        //Debug.Log("Wagon health: " + _health);
    }
}
