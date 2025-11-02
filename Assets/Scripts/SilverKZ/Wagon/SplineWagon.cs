using UnityEngine;

public class SplineWagon : MonoBehaviour
{
    [SerializeField] private CatmullRomPath _path;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private bool _loop = false;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _visualModel;      // Модель повозки (дочерний объект)
    [SerializeField] private float _rayHeight = 2f;       // Высота, с которой пускаем луч вниз
    [SerializeField] private float _alignSpeed = 10f;     // Скорость выравнивания по рельефу
    [SerializeField] private float _heightOffset = 0.8f;  // Сколько приподнять над землёй
    //[SerializeField] private float _pickupCooldown = 0.5f;

    private int _currentSegment = 0;
    private float _t = 0f;
    private Rigidbody _rb;
    private Vector3 _smoothNormal;
    private bool _isMove = false;
    //private float _lastPickupTime;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _isMove = true;
    }

    private void FixedUpdate()
    {
        if (_path == null || _path.points.Length < 4 || _currentSegment >= _path.points.Length - 1 || _isMove == false)
            return;

        // движение по сплайну
        _t += (_speed / Vector3.Distance(
            _path.points[_currentSegment].position,
            _path.points[_currentSegment + 1].position)) * Time.fixedDeltaTime;

        if (_t > 1f)
        {
            _t = 0f;
            _currentSegment++;
            if (_currentSegment >= _path.points.Length - 3)
            {
                if (_loop) _currentSegment = 0;
                else return;
            }
        }

        Vector3 splinePos = _path.GetPoint(_t, _currentSegment);
        Vector3 nextPos = _path.GetPoint(_t + 0.05f, _currentSegment);
        Vector3 moveDir = (nextPos - splinePos).normalized;

        // Raycast для выравнивания по рельефу
        if (Physics.Raycast(splinePos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 10f, _groundMask))
        {
            Vector3 groundNormal = hit.normal;
            _smoothNormal = Vector3.Lerp(_smoothNormal, groundNormal, Time.fixedDeltaTime * 8f);
            Quaternion targetRot = Quaternion.LookRotation(moveDir, _smoothNormal);

            // Двигаем физически корректно
            _rb.MovePosition(Vector3.Lerp(_rb.position, hit.point + groundNormal * _heightOffset, 0.9f));
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, Time.fixedDeltaTime * _alignSpeed));
        }
        else
        {
            // Если под повозкой нет земли
            _rb.MovePosition(splinePos);
        }

        Debug.DrawRay(splinePos, Vector3.down * (_rayHeight * 2f), Color.red);
    }
    /*
    public void Pickup()
    {
        if (Time.time - _lastPickupTime < _pickupCooldown) return;

        _lastPickupTime = Time.time;
        _isMove = !_isMove;
    }
    */
}
