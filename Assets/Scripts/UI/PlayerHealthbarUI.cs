using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbarUI : MonoBehaviour
{
    [SerializeField] private Image _barImage;

    public void Init()
    {
        PlayerHealth player = GameplayUI.Instance.PlayerHealth;
        player.OnHealthUpdated += OnPlayerHealthUpdated;
        player.OnMaxHealthUpdated += OnPlayerMaxHealthUpdated;
        UpdateImage(player.Value, player.MaxValue);
    }

    private void OnPlayerMaxHealthUpdated(PlayerHealth player)
    {
        UpdateImage(player.Value, player.MaxValue);
    }

    private void OnPlayerHealthUpdated(PlayerHealth player)
    {
        UpdateImage(player.Value, player.MaxValue);
    }

    private void UpdateImage(float value, float maxValue)
    {
        _barImage.fillAmount = Mathf.Clamp01(value / maxValue);
    }
}
