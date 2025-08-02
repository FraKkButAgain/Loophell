using UnityEngine;

public static class PlayerLastPosition
{
    private static Vector3 _nextSpawnPosition = Vector3.zero;

    public static Vector3 NextSpawnPosition
    {
        get { return _nextSpawnPosition; }
    }

    public static void SetNextSpawnPosition(Vector3 position)
    {
        _nextSpawnPosition = position;
        Debug.Log($"Spawn position set to: {_nextSpawnPosition}"); // Optional debug
    }
}