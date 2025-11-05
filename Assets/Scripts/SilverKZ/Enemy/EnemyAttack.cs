using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _speedRun = 3.0f;
    [SerializeField] private float _attackRange = 2.2f;
    [SerializeField] private float _attackCooldown = 1.0f;
    [SerializeField] private int _damage = 5;

    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _isAttack;
    private bool _isRun;
    private Wagon _wagon;
    private Player _player;
    private GameObject _target;
    private float _distPlayer;
    private float _distWagon;
    private float _distTarget;
    private float _lastAttackTime;
    private float _health;

    public static Action<GameObject> onEnemyDeath;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _isAttack = false;
        _isRun = true;
        _agent.speed = _speedRun;
        _health = _maxHealth;
    }

    private void Update()
    {
        if (_wagon == null || _player == null) 
            return;

        _isRun = true;
        Ñhasing();

        _animator.SetBool("Run", _isRun);
        _animator.SetBool("Attack", _isAttack);
    }

    public void Init(Wagon wagon, Player player)
    {
        _player = player;
        _wagon = wagon;
        _isAttack = false;
        _health = _maxHealth; 
    }

    public void Damage()
    {
        if (_distWagon < _attackRange)
        {
            _wagon.TakeDamage(_damage);
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
        onEnemyDeath?.Invoke(gameObject);
        //Destroy(gameObject);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
