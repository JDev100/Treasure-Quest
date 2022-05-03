using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSetup : MonoBehaviour
{
    [Header("Level Demographics")]
    public Vector2 playerStartLocation;

    public GameObject tileMap;
    //  public Tilemap crumblingPlatformTilemap;
    public Tile[] crumblingPlatformSet;
    public GameObject boundaryManager;

    [Header("Soundtrack")]
    public Sound levelSong;
    public Sound gameOverSound;

    [Header("Interactables Setup")]
    public List<Interactable> interactables;

    /***********************************************/
    /*          GIZMOS AND DEBUG                   */
    /***********************************************/
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float size = 0.3f;

        // float xOffset = (direction == Direction.right) ? 1 : -1;

        // GetComponent<SpriteRenderer>().s

        Gizmos.DrawLine(playerStartLocation - Vector2.up * size, playerStartLocation + Vector2.up * size);
        Gizmos.DrawLine(playerStartLocation - Vector2.left * size, playerStartLocation + Vector2.left * size);
    }
}
public class Interactable
{
    public GameObject gameObject;
    public Vector3 location;

    public Interactable(GameObject _gameObject, Vector3 _location)
    {
        gameObject = _gameObject;
        location = _location;
    }
}