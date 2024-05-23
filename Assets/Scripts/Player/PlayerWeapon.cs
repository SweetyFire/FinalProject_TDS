using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : CreatureWeapon
{
    [Header("Player")]
    [SerializeField] private GameObject _shootArea;
    [SerializeField] private SpriteRenderer _shootAreaSprite;

    private AbilityCaster _abilityCaster;

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (GameManager.GamePaused || GameManager.Instance.IsGameOver) return;

        if (ctx.canceled)
        {
            AttackEnd();
            DisableShootArea();
        }
        else if (!ctx.started)
        {
            if (!_abilityCaster.CanCastAndAttack())
                _abilityCaster.InterruptAbilityCasting();

            AttackStart();
            EnableShootArea();
        }
    }

    public void DisableShootArea()
    {
        if (_shootArea.activeSelf)
            _shootArea.SetActive(false);
    }

    protected override void InitComponents()
    {
        base.InitComponents();
        _abilityCaster = GetComponent<AbilityCaster>();
    }

    protected override void SetWeapon(Weapon weapon)
    {
        base.SetWeapon(weapon);
        UpdateShootAreaScale();
    }

    private void EnableShootArea()
    {
        if (!_shootArea.activeSelf)
            _shootArea.SetActive(true);
    }

    private void UpdateShootAreaScale()
    {
        Vector3 curScale = _shootArea.transform.localScale;
        Vector3 newScale = new(curScale.x, CalculateShootAreaScaleY(), curScale.z);
        _shootArea.transform.localScale = newScale;
    }

    private float CalculateShootAreaScaleY()
    {
        return _weapon.AttackDistance * _shootAreaSprite.sprite.pixelsPerUnit;
    }
}
