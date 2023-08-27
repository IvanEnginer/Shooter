using UnityEngine;

public class EnemyCharacter : Character
{
    [SerializeField] private Transform _head;

    [SerializeField] private Transform _body;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private Vector3 _offsetZ = new Vector3(0, 0, 0.5f);
    [SerializeField] private float _offsetColader = 1f;
    [SerializeField] private Vector3 _shiftCenterColader = new Vector3(0, 0.5f, 0);

    public Vector3 TargetPosition { get; private set; } = Vector3.zero;
    private float _velosityMagnitude = 0;

    private void Start()
    {
        TargetPosition = transform.position;
    }

    private void Update()
    {
        if(_velosityMagnitude >0.1f)
        {
            float maxDistance = _velosityMagnitude * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, maxDistance);
        }
        else
        {
            transform.position = TargetPosition;
        }

    }

    public void SetSitState(bool isSit)
    {
        if(isSit)
        {
            SitDown();
        }
        else
        {
            SitUp();
        }
    }

    public void SetSpeed(float value) => speed = value;

    public void SetMovement(in Vector3 position, in Vector3 velocity, in float averageInterval) 
    {
        TargetPosition = position + (velocity * averageInterval);
        _velosityMagnitude = velocity.magnitude;

        this.velocity = velocity;
    }

    public void SetRotateX(float value)
    {
        _head.localEulerAngles = new Vector3(value, 0, 0);
    }

    public void SetRotateY(float value)
    {
        transform.localEulerAngles = new Vector3(0, value, 0);
    }

    public void SitDown()
    {
        _head.localPosition = _head.localPosition - _offsetZ;
        _body.localPosition = _body.localPosition - _offsetZ;
        _collider.height = _collider.height - _offsetColader;
        _collider.center = _collider.center - _shiftCenterColader;
    }

    public void SitUp()
    {
        _head.localPosition = _head.localPosition + _offsetZ;
        _body.localPosition = _body.localPosition + _offsetZ;
        _collider.height = _collider.height + _offsetColader;
        _collider.center = _collider.center + _shiftCenterColader;
    }
}
