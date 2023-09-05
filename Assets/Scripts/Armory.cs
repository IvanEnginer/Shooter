using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armory : MonoBehaviour
{
    public List<PlayerGun> _guns = new List<PlayerGun>();

    private int _currentWerapon = 0;
    public void SetFerstWeapon()
    {
        _currentWerapon = 0;
    }

    public void SetSecondWeapon()
    {
        _currentWerapon = 1;
    }

    public int GetNumberCurrentWeapon()
    {
        return _currentWerapon;
    }
}
