using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
//using Jesus.ManagerSystems;

[RequireComponent(typeof(SoundPlayer))]
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;

    public LevelSetup currentLevel;
    GameObject levelObject;

    [Header("Crumbling")]
    [Range(0, 3)]
    public float crumbleTime;
    HashSet<Vector3Int> currentCrumbleCells = new HashSet<Vector3Int>();
    float nextTime;
    [Range(0, 4)]
    public float returnTime;
    Tilemap crumblingTilemap;
    Tile[] crumblingTiles;


    [Header("Camera")]
    public GameObject mainCamera;

    [Header("Player Demographics")]
    public GameObject player;


    [Header("Hurt Timer")]
    [Header("Game Over Specifications")]
    public float hurtTime = 2f;

    [Header("Knockback force")]
    [Header("Effects")]
    public GameObject hurtEffect;
    private float hurtTimer;
    bool isGameover;
    bool isKnock = false;

    [Header("Audio")]
    SoundPlayer soundPlayer;
    bool levelAudioStart = false;

    Vector2 currentVelocity;

    [Header("Essential Components")]
    private PlayerController2D playerController;
    InputManager inputManager;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    /***********************************************/
    /***********************************************/
    /*              START FUNCTION                 */
    /***********************************************/
    /***********************************************/
    void Start()
    {
        // //Instantiate objects
        //if 
        player = Instantiate(player);
        mainCamera = Instantiate(mainCamera);
        soundPlayer = GetComponent<SoundPlayer>();

        // //Set components
        inputManager = FindObjectOfType<InputManager>();
        playerController = player.GetComponent<PlayerController2D>();

        HidePlayer();

        //Start level
        //StartLevel(currentLevel);

        //Keep player and camera alive
        DontDestroyOnLoad(player);
        DontDestroyOnLoad(mainCamera);
    }

    /***********************************************/
    /***********************************************/
    /*              UPDATE FUNCTION                */
    /***********************************************/
    /***********************************************/
    void Update()
    {
        if (isGameover)
        {
            if (hurtTimer <= 0)
            {
                isGameover = false;
                RestartLevel();


            }
            else
            {
                hurtTimer -= Time.deltaTime;

                // if (knockBackTimer <= 0)
                // {
                //     if (isKnock)
                //     {
                //         Invoke("TriggerEffect", .2f);
                //         isKnock = false;
                //     }
                // }
                // else
                // {
                //     knockBackTimer -= Time.deltaTime;
                // }
            }
        }
    }

    /***********************************************/
    /***********************************************/
    /*              LEVEL MANAGEMENT               */
    /***********************************************/
    /***********************************************/
    public void StartLevel() {
        Debug.Log("Level Start");
        StartLevel(currentLevel);
    }
    public void StartLevel(LevelSetup level)
    {
        if (GameplayData.instance.playerStartLocation != Vector2.zero)
            player.transform.position = GameplayData.instance.playerStartLocation;
        else
            player.transform.position = level.playerStartLocation;
        player.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1f);


        player.GetComponent<Animator>().SetTrigger("Start");
        player.GetComponent<Animator>().SetBool("Dead", false);

        if (FindObjectOfType<BoundaryManager>())
            FindObjectOfType<BoundaryManager>().LockToBoundary();
        mainCamera.transform.position = player.transform.position;

        //SET AUDIO
        if (!levelAudioStart)
        {
            if (GameplayData.instance.currentSong != null)
                if (level.levelSong.clip != GameplayData.instance.currentSong)
                    level.levelSong.clip = GameplayData.instance.currentSong;

            //Debug.Log("Floomp");
            //StartCoroutine(SongDelay(0.01f, level.levelSong));
            StartCoroutine(SongDelay(0.01f, level.levelSong));
            //soundPlayer.PlaySound(level.levelSong);
            levelAudioStart = true;
        }

        inputManager.EnableInput();

        playerController.isGameOver = false;
        playerController.gravityScale = 1;
        playerController.physicSim.active = false;

        if (levelObject == null)
        {
            levelObject = Instantiate(level.tileMap);
            crumblingTilemap = levelObject.transform.GetChild(5).gameObject.GetComponentInChildren<Tilemap>();
            crumblingTiles = level.crumblingPlatformSet;
            nextTime = crumbleTime / (float)crumblingTiles.Length;
        }
        else
        {
            if (levelObject != level)
            {
                Destroy(levelObject);
                levelObject = Instantiate(level.tileMap);
                crumblingTilemap = levelObject.transform.GetChild(5).gameObject.GetComponentInChildren<Tilemap>();
                crumblingTiles = level.crumblingPlatformSet;
                nextTime = crumbleTime / (float)crumblingTiles.Length;
            }
        }
    }

    public void PlayerGameOver()
    {
        if (!isGameover)
        {
            EffectsManager.instance.MuffleSound();
            soundPlayer.PlaySound(currentLevel.gameOverSound);
            player.GetComponent<MeleeAttack>().Reset();
            Vector2 currentSpeed = playerController.collisionInfo.moveAmountOld;
            if (currentSpeed.x == 0)
            {
                currentSpeed = (player.GetComponent<Player>().facingRight) ? new Vector2(1, 0) : new Vector2(-1, 0);
            }

            PlayerGameOver(currentSpeed);
        }
    }

    public void PlayerGameOver(Vector2 velocity)
    {


        hurtTimer = hurtTime;
        Debug.Log("GameOver");
        inputManager.DisableInput();

        //GameObject sound = Instantiate(hurtSound);
        //Destroy(sound, 3f);


        player.GetComponent<Animator>().SetTrigger("Hurt");
        TriggerEffect();

        isGameover = true;
        isKnock = true;

        mainCamera.GetComponent<CameraEffects>().Shake();
    }

    public void HidePlayer()
    {
        inputManager.DisableInput();
        player.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
    }

    /***********************************************/
    /***********************************************/
    /*              CRUMBLING PLATFORM             */
    /***********************************************/
    /***********************************************/
    public void StartCrumble(Vector3 location)
    {
        Vector3Int nearCell = crumblingTilemap.LocalToCell(location);

        if (!currentCrumbleCells.Contains(nearCell))
        {
            currentCrumbleCells.Add(nearCell);
            StartCoroutine(CrumbleAtLocation(nearCell, crumblingTiles[0]));
        }
    }
    IEnumerator CrumbleAtLocation(Vector3Int origin, Tile tile)
    {
        yield return new WaitForSeconds(nextTime);
        tile = GetNextTile(tile);
        crumblingTilemap.SetTile(origin, tile);
        if (tile != null)
            StartCoroutine(CrumbleAtLocation(origin, tile));
        else
            StartCoroutine(ReturnCrumblePlatform(origin));
    }

    IEnumerator ReturnCrumblePlatform(Vector3Int origin)
    {
        yield return new WaitForSeconds(returnTime);
        crumblingTilemap.SetTile(origin, crumblingTiles[0]);
        currentCrumbleCells.Remove(origin);
    }

    Tile GetNextTile(Tile tile)
    {

        for (int i = 0; i < crumblingTiles.Length; i++)
        {
            if (tile == crumblingTiles[i])
            {
                if (i + 1 < crumblingTiles.Length)
                    return crumblingTiles[i + 1];
                else
                {
                    return null;
                }
            }
        }
        return null;
    }

    /***********************************************/
    /***********************************************/
    /*              GAME OVER HELPER FUNCTIONS     */
    /***********************************************/
    /***********************************************/

    // void TriggerKnockBack()
    // {

    //     //playerBody.velocity = currentVelocity;
    //     playerController.Move(currentVelocity.normalized * knockbackForce * Time.deltaTime, Vector2.zero);
    //     Invoke("StopKnockBack", knockBackTimer);
    // }

    // void StopKnockBack()
    // {
    //     //playerController.drag = 4f;
    //     Invoke("TriggerEffect", hurtTime / 2);
    // }

    void TriggerEffect()
    {
        Debug.Log("Triggering Effect");
        GameObject gameObject = ObjectPoolManager.instance.GetItem(hurtEffect, player.transform.position);
        ObjectPoolManager.instance.ClearObject(gameObject, 3f);
        //player.GetComponent<Animator>().SetBool("Dead", true);
        player.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
    }

    void RestartLevel()
    {
        //Should be refactored to an event
        StartLevel(currentLevel);
    }

    /***********************************************/
    /***********************************************/
    /*              AUDIO HELPER FUNCTIONS         */
    /***********************************************/
    /***********************************************/

    IEnumerator SongDelay(float delay, Sound sound)
    {
        //Debug.Log("Flim");
        yield return new WaitForSeconds(delay);
        //Debug.Log("Flam");
        soundPlayer.PlaySound(sound);
    }
}
