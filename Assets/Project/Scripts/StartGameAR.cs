using System;
using System.Collections;
using System.Collections.Generic;
using Niantic.Lightship.SharedAR.Colocalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAR : MonoBehaviour
{
    [SerializeField] private SharedSpaceManager sharedSpaceManager;
    [SerializeField] private Texture2D targetTexture;
    [SerializeField] private float targetImageSize;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;
    [SerializeField] private GameObject arenaPrefab;

    private const int maxAmountOfClientsPerRoom = 2;
    private string roomName = "TestRoom";
    private GameObject spawnedArena;
    private bool isHost;

    public static event Action OnStartSharedSpaceHost;
    public static event Action OnJoinedSharedSpaceClient;
    public static event Action OnStartGame;
    public static event Action OnStartSharedSpace;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        sharedSpaceManager.sharedSpaceManagerStateChanged += SharedSpaceManagerOnSharedSpaceManagerStateChanged;

        StartGameButton.onClick.AddListener(StartGame);
        CreateRoomButton.onClick.AddListener(CreateGameHost);
        JoinRoomButton.onClick.AddListener(JoinGameClient);

        StartGameButton.interactable = false;

        BlitImageForColocalization.OnTextureRendered += BlitImageForColocalizationOnTextureRendered;
    }

    private void OnDestroy()
    {
        sharedSpaceManager.sharedSpaceManagerStateChanged -= SharedSpaceManagerOnSharedSpaceManagerStateChanged;
        BlitImageForColocalization.OnTextureRendered -= BlitImageForColocalizationOnTextureRendered;
    }

    private void BlitImageForColocalizationOnTextureRendered(Texture2D texture)
    {
        SetTargetImage(texture);
        StartSharedSpace();
    }

    void SetTargetImage(Texture2D texture2D)
    {
        targetTexture = texture2D;
    }

    private void SharedSpaceManagerOnSharedSpaceManagerStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs obj)
    {
        if (!obj.Tracking)
            return;

        StartGameButton.interactable = true;
        CreateRoomButton.interactable = false;
        JoinRoomButton.interactable = false;
    }

    private void StartGame()
    {
        OnStartGame?.Invoke();

        if (isHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void SpawnArena()
    {
        if (spawnedArena != null) return;

        var origin = sharedSpaceManager.SharedArOriginObject;
        if (origin != null)
        {
            spawnedArena = Instantiate(arenaPrefab, origin.transform.position, origin.transform.rotation);
            var netObj = spawnedArena.GetComponent<NetworkObject>();
            if (netObj != null)
                netObj.Spawn();
            else
                Debug.LogWarning("Arena prefab is missing NetworkObject!");
        }
        else
        {
            Debug.LogWarning("Shared AR origin not ready; cannot spawn arena.");
        }
    }

    private void StartSharedSpace()
    {
        OnStartSharedSpace?.Invoke();

        if (sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.MockColocalization)
        {
            var mockTrackingArgs = ISharedSpaceTrackingOptions.CreateMockTrackingOptions();
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                maxAmountOfClientsPerRoom,
                "MockColocalizationDemo"
            );

            sharedSpaceManager.StartSharedSpace(mockTrackingArgs, roomArgs);
        }
        else if (sharedSpaceManager.GetColocalizationType() == SharedSpaceManager.ColocalizationType.ImageTrackingColocalization)
        {
            var imageTrackingOptions = ISharedSpaceTrackingOptions.CreateImageTrackingOptions(targetTexture, targetImageSize);
            var roomArgs = ISharedSpaceRoomOptions.CreateLightshipRoomOptions(
                roomName,
                maxAmountOfClientsPerRoom,
                "ImageColocalization"
            );

            sharedSpaceManager.StartSharedSpace(imageTrackingOptions, roomArgs);
        }
    }

    private void CreateGameHost()
    {
        isHost = true;
        OnStartSharedSpaceHost?.Invoke();
        StartSharedSpace();
    }

    private void JoinGameClient()
    {
        isHost = false;
        OnJoinedSharedSpaceClient?.Invoke();
        StartSharedSpace();
    }
}
