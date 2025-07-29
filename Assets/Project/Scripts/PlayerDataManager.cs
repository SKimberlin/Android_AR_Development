using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance;
    private NetworkList<PlayerData> allPlayerData;

    private float startHealth = 100;


    public event Action<ulong> OnPlayerDeath;
    public event Action<ulong> OnPlayerHealthChange;

    private void Awake()
    {
        allPlayerData = new NetworkList<PlayerData>();

        if (Instance == null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
    }

    public void AddPlacedPlayer(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientId == id)
            {
                PlayerData newData = new PlayerData(
                    allPlayerData[i].clientId,
                    allPlayerData[i].health,
                    true
                );

                allPlayerData[i] = newData;
            }
        }
    }

    public bool GetHasPlacerPlaced(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientId == id)
            {
                return allPlayerData[i].playerPlaced;
            }
        }
        return false;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AddNewClientToList(NetworkManager.LocalClientId);
        }
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddNewClientToList;
        PunchData.OnHitPlayer += PunchHitPlayer;
    }

    public float GetPlayerHealth(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count;i++)
        {
            if (allPlayerData[i].clientId == id) return allPlayerData[i].health;
        }

        return default;
    }

    private void PunchHitPlayer((ulong, ulong) tuple)
    {
        if (!IsServer) return;

        for (int i = 0; i < allPlayerData.Count;i++)
        {
            if (allPlayerData[i].clientId == tuple.Item2)
            {
                int lifeToReduce = 20;
                PlayerData playerData = new PlayerData(
                    allPlayerData[i].clientId,
                    allPlayerData[i].health - lifeToReduce,
                    allPlayerData[i].playerPlaced
                    );

                OnPlayerHealthChange?.Invoke(tuple.Item2);

                if (playerData.health < 0) OnPlayerDeath?.Invoke(tuple.Item2);

                allPlayerData[i] = playerData;
                break;
            }
        }
    }

    void AddNewClientToList(ulong clientId)
    {
        if (!IsServer) return;

        foreach (PlayerData playerData in allPlayerData)
        {
            if (playerData.clientId == clientId) return;
        }

        PlayerData newPlayerData = new PlayerData();
        newPlayerData.clientId = clientId;
        newPlayerData.health = startHealth;
        newPlayerData.playerPlaced = false;

        if (allPlayerData.Contains(newPlayerData)) return;
        allPlayerData.Add(newPlayerData);
    }

    void PrintAllPlayerList()
    {
        foreach (PlayerData playerData in allPlayerData)
        {
            Debug.Log("Player Id => " +  playerData.clientId + " has been placed => " + playerData.playerPlaced + ". Called by " + NetworkManager.Singleton.LocalClientId);
        }
    }
}
