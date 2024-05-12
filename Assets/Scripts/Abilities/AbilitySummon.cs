using UnityEngine;

public class AbilitySummon : AbilityBase
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Summonable _summonPrefab;

    public override void Activate()
    {
        Summonable summonable = Instantiate(_summonPrefab, _spawnPoint.position, _spawnPoint.rotation);
        summonable.Init(_owner);
        OnCompleted?.Invoke();
    }
}
