using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private PlayerGun _gun; 
    [SerializeField] private float _mouseSensetivity = 2f;
    private MultiplaerManager _multiplaerManager;

    private void Start()
    {
        _multiplaerManager = MultiplaerManager.Instance;
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool isShoot = Input.GetMouseButtonDown(0);

        bool space = Input.GetKeyDown(KeyCode.Space);

        bool isSitDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool isSitUp = Input.GetKeyUp(KeyCode.LeftControl);

        _player.SetInput(h, v, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);

        if (space) _player.Jump();

        if (isShoot && _gun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);

        if (isSitDown)
        {
            _player.SitDown();
            SendSitSate();
        }

        if (isSitUp) 
        {
            _player.SitUp();
            SendSitSate();
        }

        SendMove();
    }

    private void SendShoot(ref ShootInfo shootInfo)
    {
        shootInfo.key = _multiplaerManager.GetSessionKey();
        string json = JsonUtility.ToJson(shootInfo);

        _multiplaerManager.SendMassage("shoot", json);
    }

    private void SendMove()
    {
        _player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX,  out float rotateY);
        Dictionary<string , object> data = new Dictionary<string , object>()
        {
            { "pX", position.x },
            { "pY", position.y },
            { "pZ", position.z },
            { "vX", velocity.x },
            { "vY", velocity.y },
            { "vZ", velocity.z },
            {"rX", rotateX },
            {"rY", rotateY }
        };
        _multiplaerManager.SendMassage("move", data);
    }

    private void SendSitSate()
    {
        string json = JsonUtility.ToJson(_player._isSit);
        _multiplaerManager.SendMassage("sitState", json);
    }
}

[System.Serializable]
public struct ShootInfo
{
    public string key;
    public float pX;
    public float pY;
    public float pZ;
    public float dX;
    public float dY;
    public float dZ;
}