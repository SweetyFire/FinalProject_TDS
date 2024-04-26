using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TMP_Dropdown _qualityDropdown;

    public void Init()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        InitSettings();
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(Mathf.Clamp(level, 0, QualitySettings.count));
    }

    private void InitSettings()
    {
        _qualityDropdown.value = QualitySettings.GetQualityLevel();
    }
}
