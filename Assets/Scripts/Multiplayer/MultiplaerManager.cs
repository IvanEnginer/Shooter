using Colyseus;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MultiplaerManager : ColyseusManager<MultiplaerManager>
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private EnemyController _enemy;

    private ColyseusRoom<State> _room; 
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();

    protected override void Awake()
    {
        base.Awake();

        Instance.InitializeClient();
        Connect();
    }

    private async void Connect()
    {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"speed", _player.speed }
        };

        _room = await Instance.client.JoinOrCreate<State>("state_handler", data);

        _room.OnStateChange += OnChange;

        _room.OnMessage<string>("Shoot", ApplayShoot);

        _room.OnMessage<string>("SitState", SitChange);
    }

    private void SitChange(string jsonSitState)
    {
        _enemy.SitChange(JsonUtility.FromJson<bool>(jsonSitState));
    }

    private void ApplayShoot(string jsonShootInfo)
    {
        ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);

       if(_enemies.ContainsKey(shootInfo.key) == false) 
        {
            Debug.Log("Error, enemy not exist, enemy try shoot");
            return;
        }

        _enemies[shootInfo.key].Shoot(shootInfo);
    }

    private void OnChange(State state, bool isFirstState)
    {
        if (isFirstState == false) return;

        state.players.ForEach((string key,Player player) => {
            if (key == _room.SessionId) CreatPlayer(player);
            else CreatEnemy(key, player);
        });

        _room.State.players.OnAdd += CreatEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }

    private void CreatPlayer(Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        Instantiate(_player, position, Quaternion.identity);
    }



    private void CreatEnemy(string key ,Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        var enemy = Instantiate(_enemy, position, Quaternion.identity);
        enemy.Init(player);

        _enemies.Add(key, enemy);
    }

    private void RemoveEnemy(string key, Player player)
    {
        if(_enemies.ContainsKey(key) == false) return;

        var enemy = _enemies[key];
        enemy.Destroy();

        _enemies.Remove(key);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _room.Leave();
    }

    public void SendMassage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }

    public void SendMassage(string key, string data)
    {
        _room.Send(key, data);
    }

    public string GetSessionKey() => _room.SessionId;
}