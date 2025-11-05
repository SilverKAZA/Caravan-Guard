using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int _numberOfEnemies = 3;
    [SerializeField] private float _spawnTime = 1.0f;
    [SerializeField] private Wagon _wagon;
    [SerializeField] private Player _player;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private ObjectPool _pool;

    private int _currentEnemies;

    // Delete !!!!!!!!!!!!!!!!!!!!!!!!
    private void Start()
    {
        _currentEnemies = 0;
        SpawnEnemy();
    }

    private void OnEnable()
    {
        EnemyAttack.onEnemyDeath += EnemyDeath;
    }

    private void OnDisable()
    {
        EnemyAttack.onEnemyDeath -= EnemyDeath;
    }

    private void EnemyDeath(GameObject enemy)
    {
        _pool.ReturnObject(enemy);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Wagon wagon))
        {
            _currentEnemies = 0;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (_currentEnemies < _numberOfEnemies)
        {
            
            GameObject enemy = _pool.GetObject(); 

            if (enemy != null) 
            {
                enemy.transform.position = GetPosition();
                enemy.GetComponent<EnemyAttack>().Init(_wagon, _player);
                _currentEnemies++;
            }

            Invoke("SpawnEnemy", _spawnTime);            
        }
    }

    private Vector3 GetPosition()
    {
        int index = Random.Range(0, _spawnPoints.Length);
        return _spawnPoints[index].position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f);
    }
}
