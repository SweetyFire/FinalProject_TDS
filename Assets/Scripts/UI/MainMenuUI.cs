using UnityEngine;

public class MainMenuUI : InitializableBehavior
{
    [SerializeField] private AnimatedButton _continueButton;

    public override void Initialize()
    {
        if (!GameLoader.CanGameLoaded) return;

        _continueButton.gameObject.SetActive(true);
    }
}
