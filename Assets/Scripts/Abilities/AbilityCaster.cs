using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    [SerializeField] private Transform _abilitiesParent;
    [SerializeField] private List<AbilityCooldown> _abilitiyPrefabs;
    
    public CreatureController Controller => _controller;
    public IReadOnlyList<AbilityCooldown> Abilities => _abilities;

    private List<AbilityCooldown> _abilities = new();
    private CreatureController _controller;

    public void Init(CreatureController controller)
    {
        _controller = controller;
        InitAbilities();
    }

    public void ActivateAbility(int index)
    {
        _abilities[index].Activate();
    }

    private void InitAbilities()
    {
        foreach (AbilityCooldown ability in _abilitiyPrefabs)
        {
            AbilityCooldown abil = Instantiate(ability, _abilitiesParent);
            _abilities.Add(abil);
            abil.Init(this);
        }
    }
}
