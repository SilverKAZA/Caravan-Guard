using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] private float _range = 3f;
    [SerializeField] private LayerMask _pickupLayer;
    [SerializeField] private float _outlineWidth = 15f;
    [SerializeField] private Color _outlineColor = Color.white;
    [SerializeField] private float _loseDelay = 0.1f;

    //private int _amountCoin;
    private Camera _camera;
    private bool _isActive;
    //private Transform _outline;
    private Outline _currentOutline;
    private float _loseTimer;

    //public static Action<int> onUpdateCoin;

    private void Start()
    {
        _camera = Camera.main;
        _isActive = false;
        //_amountCoin = 0;
        //onUpdateCoin?.Invoke(_amountCoin);
    }

    private void LateUpdate()
    {


        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _range, _pickupLayer))
        {
            Outline outline = hit.collider.GetComponentInParent<Outline>();
            
            if (outline != null)
            {
                if (outline != _currentOutline)
                {
                    DisableCurrentOutline();
                    _currentOutline = outline;

                    _currentOutline.OutlineWidth = _outlineWidth;
                    _currentOutline.OutlineColor = _outlineColor;

                    _isActive = true;
                }

                _loseTimer = _loseDelay;
            }
            else
            {
                HandleLoseTimer();
                _isActive = false;
            }
        } 
        else
        {
            HandleLoseTimer();
            _isActive = false;
        }

        if (_isActive && Input.GetKey(KeyCode.E) && _currentOutline != null)
        {
            /*
            if (_outline.TryGetComponent(out Coin coin))
            {
                AudioManager.Instance.Play(AudioManager.Clip.CoinPickup);
                _outline.GetComponent<Coin>().Pickup();
                _amountCoin++;
                onUpdateCoin?.Invoke(_amountCoin);
            }
            */
            if (_currentOutline.TryGetComponent(out Wagon wagon))
            {
                _currentOutline.GetComponent<WagonAI>().Pickup();
            }
        }
    }

    private void HandleLoseTimer()
    {
        if (_currentOutline != null)
        {
            _loseTimer -= Time.deltaTime;

            if (_loseTimer <= 0f)
            {
                DisableCurrentOutline();
            }
        }
    }

    private void DisableCurrentOutline()
    {
        if (_currentOutline != null)
        {
            _currentOutline.OutlineWidth = 0f;
            _currentOutline = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, _range);
        Gizmos.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * _range);
    }
}
