using UnityEngine;

public abstract class CreatureWeapon : MonoBehaviour
{
    [Header("Creature")]
    [SerializeField] protected Transform _attachPoint;
    [SerializeField] protected Animator _animator;

    public Transform AttachPoint => _attachPoint;

    public int Team => _health.Team;
    public bool IsAttacking => _isAttacking;
    public CreatureController Controller => _controller;
    public CreatureHealth Health => _health;
    public bool DisabledAttack => _disabledAttack || _controller.IsStunned || !_health.IsAlive || !GameLoader.Instance.GameLoaded || GameManager.GamePaused;

    public Weapon Weapon => _weapon;
    protected Weapon _weapon;
    protected CreatureHealth _health;
    protected CreatureController _controller;
    private bool _disabledAttack;
    private bool _isAttacking;

    protected virtual void Start()
    {
        if (_weapon == null)
            throw new System.Exception($"{gameObject.name} doesn't have any weapon");
    }

    protected virtual void Update()
    {
        AttackUpdate();
    }

    public void Init()
    {
        InitComponents();
    }

    public virtual bool TryAddWeapon(Weapon weapon)
    {
        if (_weapon == null)
        {
            SetWeapon(weapon);
            return true;
        }
        else
            return false;
    }

    public void AttackStart()
    {
        _isAttacking = true;
    }

    public void AttackEnd()
    {
        _isAttacking = false;
    }

    public void EnableAttack()
    {
        _disabledAttack = false;
    }

    public void DisableAttack()
    {
        _disabledAttack = true;
    }

    protected virtual void InitComponents()
    {
        _health = GetComponent<CreatureHealth>();
        _controller = GetComponent<CreatureController>();
        InitWeapon();
    }

    protected void InitWeapon()
    {
        SetWeapon(_attachPoint.GetChild(0).GetComponent<Weapon>());
        _weapon.Pickup(this);
    }

    protected virtual void SetWeapon(Weapon weapon)
    {
        Weapon prevWeapon = _weapon;

        _weapon = weapon;
        _weapon.transform.SetParent(_attachPoint);
        _weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void AttackUpdate()
    {
        if (_isAttacking)
        {
            Attack();
        }
    }

    protected void Attack()
    {
        if (DisabledAttack) return;

        if (_weapon == null) return;

        if (_weapon.CanAttack)
            _animator.SetTrigger(_weapon.AnimationAttack);

        _weapon.Attack();
    }
}
