using System;
using UnityEngine;

public class Wagon : MonoBehaviour
{
    [SerializeField] private int _health = 100;

    public static Action<int> onUpdateWagonHP;

    private void Start()
    {
        onUpdateWagonHP?.Invoke(_health);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        onUpdateWagonHP?.Invoke(_health);
    }

    public void LevelComplet()
    {
        Debug.Log("Level Complet! Reward: " + _health);
    }
}
