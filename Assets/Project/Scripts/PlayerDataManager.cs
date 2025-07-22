using Unity.Netcode;
using UnityEngine;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance;
    private NetworkList<PlayerData> allPlayerData;

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
