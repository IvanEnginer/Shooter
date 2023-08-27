using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string _grounded = "Grounded";
    private const string _speed = "Speed";

    [SerializeField] private Animator _animation;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character _character;

    private void Update()
    {
        Vector3 localVelocity = _character.transform.InverseTransformVector(_character.velocity);
        float speed = localVelocity.magnitude / _character.speed;
        float sign = Mathf.Sign(localVelocity.z);

        _animation.SetFloat(_speed, speed * sign);
        _animation.SetBool(_grounded, _checkFly.IsFly == false);
    }
}
