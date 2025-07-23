using Niantic.Lightship.SharedAR.Colocalization;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StartGameAR : MonoBehaviour
{
    [SerializeField]
    private SharedSpaceManager sharedSpaceManager;
    const int maxAmountOfClientsPerRoom = 2;

    [SerializeField] private Texture2D targetTexture;
    [SerializeField] private float targetImageSize;
    private string roomName = "TestRoom";

    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button CreateRoomButton;
    [SerializeField] private Button JoinRoomButton;

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
    }

    private void SharedSpaceManagerOnSharedSpaceManagerStateChanged(SharedSpaceManager.SharedSpaceManagerStateChangeEventArgs obj)
    {
        if (obj.Tracking)
        {
            StartGameButton.interactable = true;
            CreateRoomButton.interactable = false;
            JoinRoomButton.interactable = false;
        }
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

            sharedSpaceManager.StartSharedSpace( mockTrackingArgs, roomArgs );
            return;
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
            return;
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
