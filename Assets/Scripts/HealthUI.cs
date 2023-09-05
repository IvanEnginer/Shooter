using UnityEngine;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private RectTransform _filledImage;
    [SerializeField] private float _defoultWith;

    private void OnValidate()
    {
        _defoultWith = _filledImage.sizeDelta.x;
    }

    public void UpdateHealth(float max, int current)
    {
        float percent = current / max;
        _filledImage.sizeDelta = new Vector3(_defoultWith * percent, _filledImage.sizeDelta.y);
    }
}
