using UnityEngine;

public abstract class CreatureWeapon : MonoBehaviour
{
    [SerializeField] protected Transform _attachPoint;
    public Transform AttachPoint => _attachPoint;

    public int Team => _health.Team;
    public bool IsAttacking => _isAttacking;
    public CreatureController Controller => _controller;
    public CreatureHealth Health => _health;

    protected Weapon _weapon;
    protected CreatureHealth _health;
    protected CreatureController _controller;
    private bool _isAttacking;

    private void Awake()
    {
        InitComponents();
    }

    private void Update()
    {
        AttackUpdate();
    }

    public bool TryAddWeapon(Weapon weapon)
    {
        if (_weapon == null)
        {
            _weapon = weapon;
            _weapon.transform.SetParent(_attachPoint);
            _weapon.transform.SetLocalPositionAndRotation(Vector3.zero,Quaternion.identity);
            return true;
        }
        else
        {

            return false;
        }
    }

    public void AttackStart()
    {
        _isAttacking = true;
    }

    public void AttackEnd()
    {
        _isAttacking = false;
    }

    protected virtual void InitComponents()
    {
        _health = GetComponent<CreatureHealth>();
        _controller = GetComponent<CreatureController>();
    }

    private void AttackUpdate()
    {
        if (_isAttacking)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (_weapon == null) return;
        _weapon.Attack();
    }
}
