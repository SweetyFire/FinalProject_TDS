using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get; private set; }

    public PlayerHealth PlayerHealth => _player;
    [SerializeField] private PlayerHealth _player;
    [SerializeField] private PlayerHealthbarUI _playerHealthbar;

    private void Awake()
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

        InitUI();
    }

    private void InitUI()
    {
        _playerHealthbar.Init();
    }
}
