using UnityEngine;
using UnityEngine.UI;

public class LossCounter : MonoBehaviour
{
    [SerializeField] private Text _text;
    private int _enemyLosse;
    private int _playerLosse;

    public void SetEnemyLoss(int value)
    {
        _enemyLosse = value;
        UpdateText();
    }

    public void SetPlayerLoss(int value)
    {
        _playerLosse = value;
        UpdateText();
    }

    private void UpdateText()
    {
        _text.text = $"{_playerLosse} : {_enemyLosse}";
    }
}
