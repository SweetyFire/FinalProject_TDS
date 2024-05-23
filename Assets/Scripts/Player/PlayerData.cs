using UnityEngine;

public class PlayerData
{
    public Vector3 position;
    public Quaternion rotation;
    public float health;

    private const string POSITION_NAME = "playerPosition";
    private const string ROTATION_NAME = "playerRotation";
    private const string HEALTH_NAME = "playerHealth";

    public static void SetValues(PlayerController player)
    {
        SaveSystem.SetFormatValue(POSITION_NAME, player.transform.position);
        SaveSystem.SetFormatValue(ROTATION_NAME, player.transform.rotation);
        SaveSystem.SetConvertibleValue(HEALTH_NAME, player.Health.Value);
    }

    public void Load(PlayerController player)
    {
        string outValue;
        if (!SaveSystem.TryGetString(POSITION_NAME, out outValue)) return;
        if (!VectorExtensions.TryParse(outValue, out position)) return;

        if (!SaveSystem.TryGetString(ROTATION_NAME, out outValue)) return;
        if (!QuaternionExtensions.TryParse(outValue, out rotation)) return;

        if (!SaveSystem.TryGetFloat(HEALTH_NAME, out health)) return;

        player.LoadProgress(this);
    }
}
