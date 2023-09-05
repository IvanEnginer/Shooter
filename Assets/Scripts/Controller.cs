using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour
{
    [SerializeField] private float _restartDelay = 3f;
    [SerializeField] private PlayerCharacter _player;
    //[SerializeField] private PlayerGun _gun;
    [SerializeField] Armory _armory;
    [SerializeField] private int _numberCurrentWeapon = 0;
    [SerializeField] private float _mouseSensetivity = 2f;
    private MultiplaerManager _multiplaerManager;
    private bool _hold = false;

    private void Start()
    {
        _multiplaerManager = MultiplaerManager.Instance;
    }

    private void Update()
    {
        if (_hold) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool isShoot = Input.GetMouseButtonDown(0);

        bool space = Input.GetKeyDown(KeyCode.Space);

        bool isSitDown = Input.GetKeyDown(KeyCode.LeftControl);
        bool isSitUp = Input.GetKeyUp(KeyCode.LeftControl);

        bool isFerstWeapon = Input.GetKeyDown(KeyCode.Alpha1);
        bool isSecondWeapon = Input.GetKeyDown(KeyCode.Alpha2);

        _player.SetInput(h, v, mouseX * _mouseSensetivity);
        _player.RotateX(-mouseY * _mouseSensetivity);

        if (space) _player.Jump();

        if (isFerstWeapon)
        {
            _armory.SetFerstWeapon();
        }

        if (isSecondWeapon)
        {
            _armory.SetSecondWeapon();
        }

        if (isShoot && _armory._guns[_armory.GetNumberCurrentWeapon()].TryShoot(out ShootInfo shootInfo))SendShoot(ref shootInfo);

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

    public void Restart(string jsonRestartInfo)
    {
       RestartInfo info =  JsonUtility.FromJson<RestartInfo>(jsonRestartInfo);
       StartCoroutine(Hold());
        _player.transform.position = new Vector3(info.x, 0, info.z);
        _player.SetInput(0, 0, 0);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "pX", info.x },
            { "pY", 0 },
            { "pZ", info.z },
            { "vX", 0 },
            { "vY", 0 },
            { "vZ", 0 },
            { "rX", 0 },
            { "rY", 0 }
        };

        _multiplaerManager.SendMassage("move", data);
    }

    private IEnumerator Hold()
    {
        _hold = true;
        yield return new WaitForSecondsRealtime(_restartDelay);
        _hold = false;
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

[Serializable]
public struct RestartInfo
{
    public float x;
    public float z;
}