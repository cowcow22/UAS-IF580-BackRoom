using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI; // Add this line
using Unity.AI.Navigation;
public enum GenerationState
{
    Idle,
    GeneratingRooms,
    GeneratingLighting,

    GeneratingSpawn,
    GeneratingExit,

    GeneratingBarrier,
    BakingNavMesh // Add this state
}
public class GenerationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform WorldGrid;
    [SerializeField] List<GameObject> RoomTypes;
    [SerializeField] List<GameObject> LightTypes;
    [SerializeField] int mapSize = 16; // Size of the map
    // [SerializeField] Slider MapSizeSlider, EmptinessSlider, BrightnessSlider;
    // [SerializeField] Button GenerateButton;
    [SerializeField] GameObject E_Room;
    [SerializeField] GameObject B_Room;
    [SerializeField] GameObject SpawnRoom, ExitRoom;
    [SerializeField] NavMeshSurface navMeshSurface; // Add this line
    [SerializeField] GameObject DoggyObject, PeanutObject, SCP096;
    public List<GameObject> GeneratedRooms;
    [SerializeField] GameObject PlayerObject, MainCameraObject;


    [Header("Settings")]
    public int mapEmptiness;
    public int mapBrightness;
    private int mapSizeSquare; // square root of the map size
    private float currentPosX, currentPosZ, currentPosTracker, currentRoom; // These will keep track of the current position of the room to be generated
    public float roomSize = 7;
    private Vector3 currentPos; // This will keep track of the position of the room to be generated
    public GenerationState currentState;



    private void Update()
    {
        mapSize = (int)Mathf.Pow(4, 4);
        mapSizeSquare = (int)Mathf.Sqrt(mapSize);
        mapEmptiness = (int)9;
        mapBrightness = (int)4;
    }

    public void ReloadWorld() // Reload the world to generate a new one
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Start()
    {
        mapSize = (int)Mathf.Pow(4, 4);
        mapSizeSquare = (int)Mathf.Sqrt(mapSize);
        mapEmptiness = (int)9;
        mapBrightness = (int)4;
        GenerateWorld();

        SpawnPlayer();
    }

    public void GenerateWorld() // Generate the world
    {
        for (int i = 0; i < mapEmptiness; i++)
        {
            RoomTypes.Add(E_Room);
        }
        // GenerateButton.interactable = false;

        for (int state = 0; state < 7; state++)
        {
            for (int i = 0; i < mapSize; i++)
            {
                if (currentPosTracker == mapSizeSquare)
                {

                    currentPosX = 0;
                    currentPosTracker = 0;

                    currentPosZ += roomSize;
                }
                currentPos = new(currentPosX, 0, currentPosZ);

                switch (currentState)
                {
                    case GenerationState.GeneratingRooms:
                        GeneratedRooms.Add(Instantiate(RoomTypes[Random.Range(0, RoomTypes.Count)], currentPos, Quaternion.identity, WorldGrid));
                        break;

                    case GenerationState.GeneratingLighting:
                        int lightSpawn = Random.Range(-1, mapBrightness);

                        if (lightSpawn == 0)
                            Instantiate(LightTypes[Random.Range(0, LightTypes.Count)], currentPos, Quaternion.identity, WorldGrid);
                        break;

                    case GenerationState.GeneratingBarrier:
                        GenerateOutskirtsBarriers();
                        break;
                }
                currentPosTracker++;
                currentPosX += roomSize;
                currentRoom++;
            }
            NextState();


            switch (currentState)
            {
                case GenerationState.GeneratingExit:

                    int roomToReplace = Random.Range(0, GeneratedRooms.Count);
                    exitRoom = Instantiate(ExitRoom, GeneratedRooms[roomToReplace].transform.position, Quaternion.identity, WorldGrid);

                    Destroy(GeneratedRooms[roomToReplace]);

                    GeneratedRooms[roomToReplace] = exitRoom;

                    break;

                case GenerationState.GeneratingSpawn:
                    int _roomToReplace = Random.Range(0, GeneratedRooms.Count);
                    spawnRoom = Instantiate(SpawnRoom, GeneratedRooms[_roomToReplace].transform.position, Quaternion.identity, WorldGrid);

                    Destroy(GeneratedRooms[_roomToReplace]);

                    GeneratedRooms[_roomToReplace] = spawnRoom;
                    break;

                case GenerationState.BakingNavMesh:
                    // Set layer mask to only include "Navmesh" layer
                    int navMeshLayer = LayerMask.NameToLayer("Navmesh");
                    navMeshSurface.layerMask = 1 << navMeshLayer;

                    // Bake the NavMesh
                    navMeshSurface.BuildNavMesh();
                    break;
            }


        }
    }

    public GameObject spawnRoom;
    public GameObject exitRoom;

    public void SpawnPlayer()
    {
        PlayerObject.SetActive(false);

        PlayerObject.transform.position = new Vector3(spawnRoom.transform.position.x, 1.8f, spawnRoom.transform.position.z);

        int peanutSpawnRoom = Random.Range(0, GeneratedRooms.Count);
        int scp096SpawnRoom = Random.Range(0, GeneratedRooms.Count);

        DoggyObject.transform.position = new Vector3(exitRoom.transform.position.x, 1.8f, exitRoom.transform.position.z);
        DoggyObject.SetActive(true);
        PeanutObject.transform.position = new Vector3(GeneratedRooms[peanutSpawnRoom].transform.position.x, 1.8f, GeneratedRooms[peanutSpawnRoom].transform.position.z);
        PeanutObject.SetActive(true);
        SCP096.transform.position = new Vector3(GeneratedRooms[scp096SpawnRoom].transform.position.x, 1.8f, GeneratedRooms[scp096SpawnRoom].transform.position.z);
        SCP096.SetActive(true);

        PlayerObject.SetActive(true);
        MainCameraObject.SetActive(false);
    }

    public void NextState()
    {
        currentState++;

        currentRoom = 0;
        currentPosX = 0;
        currentPosZ = 0;
        currentPos = Vector3.zero;
        currentPosTracker = 0;
    }

    public void WinGame()
    {
        MainCameraObject.SetActive(true);
        PlayerObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Kalo menang

        Debug.Log("Player has exited and won the game!");
    }

    private void GenerateOutskirtsBarriers()
    {
        for (int x = -1; x <= mapSizeSquare; x++)
        {
            // Bottom row
            Instantiate(B_Room, new Vector3(x * roomSize, 0, -roomSize), Quaternion.identity, WorldGrid);
            // Top row
            Instantiate(B_Room, new Vector3(x * roomSize, 0, mapSizeSquare * roomSize), Quaternion.identity, WorldGrid);
        }

        for (int z = 0; z <= mapSizeSquare; z++)
        {
            // Left column
            Instantiate(B_Room, new Vector3(-roomSize, 0, z * roomSize), Quaternion.identity, WorldGrid);
            // Right column
            Instantiate(B_Room, new Vector3(mapSizeSquare * roomSize, 0, z * roomSize), Quaternion.identity, WorldGrid);
        }
    }

}
