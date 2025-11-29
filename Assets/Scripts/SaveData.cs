using System;
using System.Xml.Serialization;

[Serializable]
public class SaveData
{
    public int Version = 1;                         // For future migrations
    public string SavedAt = "";                     // Optional: last save time

    public PlayerData Player = new PlayerData();
    public CheckpointData Checkpoint = new CheckpointData();
    public int Score = 0;
}

[Serializable]
public class PlayerData
{
    public int Lives = 3;
    public float[] Position = new float[3];         // x, y, z
    public QuaternionData Rotation = new QuaternionData();
}

[Serializable]
public class CheckpointData
{
    public bool HasCheckpoint = false;              // NEW: prevents errors when starting new game
    public string checkpointId = "";
    public float[] checkpointPosition = new float[3];
}

[Serializable]
public class QuaternionData
{
    public float x, y, z, w;

    public QuaternionData() { }

    public QuaternionData(UnityEngine.Quaternion q)
    {
        x = q.x; y = q.y; z = q.z; w = q.w;
    }

    public UnityEngine.Quaternion ToQuaternion()
        => new UnityEngine.Quaternion(x, y, z, w);
}