using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;
using TMPro;

public class SimpleNetworkManager : MonoBehaviour
{
    public TMP_InputField ipInputField; // Assign this in the inspector
    public TMP_Text statusText;

    private NetworkDriver driver;
    private NativeList<NetworkConnection> connections;
    private bool isServer = false;
    private bool isClient = false;
    private ushort port = 7777;

    private void Awake()
    {
        // Create the driver and allocate space for connections
        driver = NetworkDriver.Create();
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }

    // ?? Call this from your Host button
    public void StartHost()
    {
        var endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = port; // choose a port

        var bindResult = driver.Bind(endpoint);
        if (driver.Bind(endpoint) != 0)
        {
            Show("Failed to bind to port " + port + " (code " + bindResult + ")");
            return;
        }
        
        driver.Listen();
        isServer = true;
        isClient = false;
        Show("Hosting on port " + port);
    }

    // ?? Call this from your Join button
    public void StartClient()
    {
        string ip = ipInputField.text;
        if (string.IsNullOrWhiteSpace(ip))
            ip = "127.0.0.1";

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
            // Server-only: accept new connections
            NetworkConnection c;
            while ((c = driver.Accept()) != default)
            {
                connections.Add(c);
                Show("Accepted a connection");
            }
        }

        // Both roles: process events for existing connections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
                continue;

            DataStreamReader stream;
            NetworkEvent.Type evt;
            while ((evt = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (evt == NetworkEvent.Type.Connect)
                {
                    Show("Connected!");
                    DataStreamWriter writer;
                    driver.BeginSend(NetworkPipeline.Null, connections[i], out writer);
                    writer.WriteInt(12345);
                    driver.EndSend(writer);
                }
                else if (evt == NetworkEvent.Type.Data)
                {
                    int value = stream.ReadInt();
                    Show("Got message: " + value);
                }
                else if (evt == NetworkEvent.Type.Disconnect)
                {
                    Show("Disconnected");
                    connections[i] = default;
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (driver.IsCreated) driver.Dispose();
        if (connections.IsCreated) connections.Dispose();
    }

    private void Show(string msg)
    {
        Debug.Log(msg);
        if (statusText) statusText.text = msg;
    }
}

