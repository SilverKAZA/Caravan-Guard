using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _capacity;

    private List<GameObject> _pool = new List<GameObject>();

    private void Awake()
    {
        Initialize();
    }

    public GameObject GetObject()
    {
        return (TryGetObject(out GameObject poolObject)) ? poolObject : null;
    }

    public void ReturnObject(GameObject poolObject)
    {
        poolObject.SetActive(false); 
    }

    private void Initialize() 
    {
        for (int i = 0; i < _capacity; i++)
        {
            GameObject spawned = Instantiate(_prefab, transform);
            spawned.SetActive(false);
            _pool.Add(spawned);
        }
    }

    private bool TryGetObject(out GameObject result)
    {
        result = _pool.FirstOrDefault(p => p.activeSelf == false);
        
        if (result != null) 
            result.SetActive(true);

        return result != null;
    }
}
