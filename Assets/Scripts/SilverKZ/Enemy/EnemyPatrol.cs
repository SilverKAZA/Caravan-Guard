using System.Collections;
using UnityEngine;
using UnityEngine.AI;

enum State { Idle, Walk }

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private State _state;
    [SerializeField] private float _health = 100f;
    [SerializeField] private float _speedWalk = 1.3f;
    [SerializeField] private float _speedRun = 3.0f;
    [SerializeField] private float _chaseRange = 10f;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _attackCooldown = 1.0f;
    [SerializeField] private int _damage = 5;
    [SerializeField] private Transform[] _patrolPoints;

    private NavMeshAgent _agent;
    private Animator _animator;
    private SphereCollider _collider;
    private Vector3[] _points;
    private int _currentIndexPoint = 0;
    private float _pointRadius = 0.5f;
    private bool _alreadyPatroling;
    private bool _isAttack;
    private bool _isWalk;
    private bool _isRun;
    private Wagon _wagon;
    private Player _player;
    private GameObject _target;
    private float _distPlayer;
    private float _distWagon;
    private float _distTarget; 
    private float _lastAttackTime;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<SphereCollider>();
        _collider.radius = _chaseRange;

        _isAttack = false;
        _isWalk = false;
        _isRun = false;

        if (_state == State.Idle || _patrolPoints.Length == 0) return;

        _agent.speed = _speedWalk;
        _alreadyPatroling = true;
        _points = new Vector3[_patrolPoints.Length];

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            _points[i] = _patrolPoints[i].position;
        }
    }

    private void Update()
    {
        if (_wagon != null || _player != null)
        {
            _isWalk = false;
            _isRun = true;
            Ñhasing();
        }
        else
        {
            if (_state == State.Idle || _patrolPoints.Length == 0) return;

            _isWalk = true;
            _isRun = false;
            Patroling();
        }

        _animator.SetBool("Walk", _isWalk);
        _animator.SetBool("Run", _isRun);
        _animator.SetBool("Attack", _isAttack);
    }

    public void Damage()
    {
        if (_distWagon < _attackRange)
        {
            _wagon.TakeDamage(_damage);
            TakeDamage(20);
        }
        else
        {
            _player.TakeDamage(_damage);
            TakeDamage(20);
        }
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);
        //onEnemyDeath?.Invoke(gameObject);
        //Destroy(gameObject);
    }

    private void Patroling()
    {
        if (Vector3.Distance(_points[_currentIndexPoint], transform.position) < _pointRadius)
        {
            _currentIndexPoint = Random.Range(0, _points.Length);
            _alreadyPatroling = true;
        }
        else if (_alreadyPatroling)
        {
            _alreadyPatroling = false;
            _agent.SetDestination(_points[_currentIndexPoint]);
        }
    }

    private void Ñhasing()
    {
        if (_isAttack) return;

        _target = TargetSelecting();
        _distTarget = GetDistance(_target);

        if (_distTarget < _attackRange)
        {
            _isRun = false;
            AttackTarget();
        }
        else
        {
            _agent.speed = _speedRun;
            _agent.SetDestination(_target.transform.position);
        }
    }

    private void AttackTarget()
    {
        if (Time.time - _lastAttackTime < _attackCooldown || _isAttack) return;

        _lastAttackTime = Time.time;
        _isAttack = true;
        Rotation();
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        StopAgentInstantly();
        yield return new WaitForSeconds(1.0f); 
        _isAttack = false;
    }

    private GameObject TargetSelecting()
    {
        GameObject target;

        if (_wagon != null && _player != null)
        {
            _distPlayer = GetDistance(_player.gameObject);
            _distWagon = GetDistance(_wagon.gameObject);
            target = (_distWagon < _distPlayer) ? _wagon.gameObject : _player.gameObject;
        }
        else
        {
            target = (_player == null) ? _wagon.gameObject : _player.gameObject;
        }

        return target;
    }

    private void StopAgentInstantly()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;
    }

    private void Rotation()
    {
        if (_target == null) return;

        Vector3 direction = _target.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            20f * Time.deltaTime
        );
    }

    private float GetDistance(GameObject target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            _player = player;
        }

        if (collision.TryGetComponent(out Wagon wagon))
        {
            _wagon = wagon;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
