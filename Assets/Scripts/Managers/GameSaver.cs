using UnityEngine;

public class GameSaver : MonoBehaviour
{
    public void SaveGame()
    {
        GameLoader.Instance.Save();
    }
}
