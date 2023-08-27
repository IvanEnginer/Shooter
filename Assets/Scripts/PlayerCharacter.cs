using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _maxHeadAngel = 90;
    [SerializeField] private float _minHeadAngel = -90;
    [SerializeField] private float _jumpForse = 500;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private float _jumpDelay = .2f;

    [SerializeField] private Transform _body;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Vector3 _offsetZ = new Vector3(0, 0, 0.5f);
    [SerializeField] private float _offsetColader = 1f;
    [SerializeField] private Vector3 _shiftCenterColader =new Vector3(0, 0.5f, 0);

    public bool _isSit {
        get { return false; }
        private set { }
    }

    private float _inputH;
    private float _inputV;
    private float _rotateY;
    private float _currentRotateX;
    private float _jumpTime; 

    private void Start()
    {
        Transform camera = Camera.main.transform;
        camera.parent = _cameraPoint;
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
    }

    public void SetInput(float h, float v, float rotateY)
    {
        _inputH = h;
        _inputV = v;
        _rotateY += _rotateY;
    }

    private void FixedUpdate()
    {
        Move();
        RotateY();
    }

    private void Move()
    {
        //Vector3 direction = new Vector3(_inputH, 0, _inputV).normalized;
        //transform.position += direction * Time.deltaTime * _speed;

        Vector3 velocity = (transform.forward * _inputV + transform.right * _inputH).normalized * _speed;
        velocity.y = _rigidbody.velocity.y;
        base.velocity = velocity;

        _rigidbody.velocity = base.velocity;
    }

    private void RotateY()
    {
        _rigidbody.angularVelocity = new Vector3(0, _rotateY, 0);
        _rotateY = 0;
    }

    public void RotateX(float value)
    {
        _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngel, _maxHeadAngel);
        _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
    }

    public void SitDown()
    {
        _isSit = true;

        _head.localPosition = _head.localPosition - _offsetZ;
        _body.localPosition = _body.localPosition - _offsetZ;
        _collider.height = _collider.height - _offsetColader;
        _collider.center = _collider.center - _shiftCenterColader;
    }

    public void SitUp()
    {
        _isSit = false;

        _head.localPosition = _head.localPosition + _offsetZ;
        _body.localPosition = _body.localPosition + _offsetZ;
        _collider.height = _collider.height + _offsetColader;
        _collider.center = _collider.center + _shiftCenterColader;
    }

    public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY)
    {
        position = transform.position;
        velocity = _rigidbody.velocity;

        rotateX = _head.localEulerAngles.x;
        rotateY = transform.eulerAngles.y;
    }

    private bool _isFly = true;

    private void OnCollisionStay(Collision collision)
    {
        var contactPoints= collision.contacts;

        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (contactPoints[i].normal.y > 0.45f) _isFly = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isFly = true;
    }

    public void Jump()
    {
        if (_checkFly.IsFly) return;
        if (Time.time - _jumpTime < _jumpDelay) return;

        _jumpForse = Time.time;
        _rigidbody.AddForce(0, _jumpForse, 0, ForceMode.VelocityChange);
    }
}
