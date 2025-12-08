using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SimpleNetworkManager : MonoBehaviour
{
    public TMP_InputField ipInputField; // Assign this in the inspector
    public TMP_Text statusText;

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    private bool isServer = false;
    private bool isClient = false;
    private ushort port = 7777;

    private int nextPlayerId = 1;
    private Dictionary<NetworkConnection, int> playerIds = new Dictionary<NetworkConnection, int>();

    public enum MessageType
    {
        PlayerTransform,
        PlayerHealth,
        PlayerScore,
        PlayerLives,
        EnemySpawn
    }

    private void Awake()
    {
        Debug.Log("NM: AWAKE");
        driver = NetworkDriver.Create();
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    private void Start()
    {
        Debug.Log("NM: START");
    }

    public void StartHost()
    {
        var endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = port;

        var result = driver.Bind(endpoint);
        Debug.Log("NM: BIND RESULT = " + result);

        if (result != 0)
        {
            Show("Failed to bind to port " + port + " (code " + result + ")");
            return;
        }
        
        driver.Listen();
        isServer = true;
        isClient = false;
        Show("Hosting on port " + port);
    }

    public void StartClient()
    {
        string ip = string.IsNullOrWhiteSpace(ipInputField != null ? ipInputField.text : null) ? "127.0.0.1" : ipInputField.text;

        var endpoint = NetworkEndpoint.Parse(ip, port);
        var connection = driver.Connect(endpoint);
        connections.Add(connection);

        isClient = true;
        isServer = false;
        Show("Connecting to " + ip);
    }

    private void Update()
    {
        driver.ScheduleUpdate().Complete();

        if (isServer)
        {
            // Accept new connections
            NetworkConnection c;
            while ((c = driver.Accept()) != default)
            {
                connections.Add(c);

                int id = nextPlayerId++;
                playerIds[c] = id;

                Show($"Accepted a connection, assigned PlayerID {id}");

                // Send a test message immediately when a client connects
                SendMessage(c, MessageType.PlayerLives, id, 3);
                SendMessage(c, MessageType.PlayerHealth, id, 100);
                SendMessage(c, MessageType.PlayerScore, id, 0);
            }
        }

        // Process events for all connections
        for (int i = 0; i < connections.Length; i++)
        {
            var connection = connections[i];
            if (!connection.IsCreated)
                continue;

            DataStreamReader stream;
            NetworkEvent.Type evt;
            while ((evt = driver.PopEventForConnection(connection, out stream)) != NetworkEvent.Type.Empty)
            {
                switch (evt)
                {
                    case NetworkEvent.Type.Connect:
                        Show("Connected to server");
                        break;

                    case NetworkEvent.Type.Data:
                        MessageType type = (MessageType)stream.ReadInt();
                        int playerId = stream.ReadInt();

                        switch (type)
                        {
                            case MessageType.PlayerTransform:
                                float x = stream.ReadFloat();
                                float y = stream.ReadFloat();
                                float z = stream.ReadFloat();
                                float rotY = stream.ReadFloat();
                                Show($"Player {playerId} moved to ({x},{y},{z}) rot {rotY}");
                                break;

                            case MessageType.PlayerHealth:
                                int health = stream.ReadInt();
                                Show($"Player {playerId} health = {health}");
                                break;

                            case MessageType.PlayerScore:
                                int score = stream.ReadInt();
                                Show($"Player {playerId} score = {score}");
                                break;

                            case MessageType.PlayerLives:
                                int lives = stream.ReadInt();
                                Show($"Player {playerId} lives = {lives}");
                                break;

                            case MessageType.EnemySpawn:
                                float ex = stream.ReadFloat();
                                float ey = stream.ReadFloat();
                                float ez = stream.ReadFloat();
                                Show($"Spawn enemy at ({ex},{ey},{ez})");
                                break;
                        }

                        if (isServer)
                        {
                            for (int j = 0; j < connections.Length; j++)
                            {
                                if (connections[j].IsCreated && connections[j] != connection)
                                {
                                    // Rebroadcast same message
                                    SendMessage(connections[j], type, playerId, 0, new Vector3(0, 0, 0));
                                }
                            }
                        }

                        break;

                    case NetworkEvent.Type.Disconnect:
                        Show("Disconnected from server");
                        connections[i] = default;
                        break;
                }
            }
        }
    }


    private void SendMessage(NetworkConnection conn, MessageType type, int playerId, int value = 0, Vector3? pos = null, float rotY = 0f)
    {
        if (!conn.IsCreated) return;

        driver.BeginSend(NetworkPipeline.Null, conn, out var writer);

        writer.WriteInt((int)type);
        writer.WriteInt(playerId);

        switch (type)
        {
            case MessageType.PlayerTransform:
                Vector3 p = pos ?? Vector3.zero;
                writer.WriteFloat(p.x);
                writer.WriteFloat(p.y);
                writer.WriteFloat(p.z);
                writer.WriteFloat(rotY);
                break;

            case MessageType.PlayerHealth:
            case MessageType.PlayerScore:
            case MessageType.PlayerLives:
                writer.WriteInt(value);
                break;

            case MessageType.EnemySpawn:
                Vector3 e = pos ?? Vector3.zero;
                writer.WriteFloat(e.x);
                writer.WriteFloat(e.y);
                writer.WriteFloat(e.z);
                break;
        }

        driver.EndSend(writer);
    }

private void OnDestroy()
    {
        if (driver.IsCreated) driver.Dispose();
        if (connections.IsCreated) connections.Dispose();
    }

    private void Show(string msg)
    {
        Debug.Log(msg);
        if (statusText != null)
            statusText.text += "\n" + msg;
    }
}

