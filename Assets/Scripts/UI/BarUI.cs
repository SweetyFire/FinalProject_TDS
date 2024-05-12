using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] private Image _barImage;

    public void UpdateValue(float value, float maxValue)
    {
        _barImage.fillAmount = Mathf.Clamp01(value / maxValue);
    }
}
