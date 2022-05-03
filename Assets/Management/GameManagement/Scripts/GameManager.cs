using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;



    // Start is called before the first frame update
    /***********************************************/
    /***********************************************/
    /*              AWAKE FUNCTION                 */
    /***********************************************/
    /***********************************************/
    void Awake()
    {
        //Singleton stuff
        if (gameManager == null)
        {
            gameManager = this;
        }
        else
        {
            Destroy(gameManager);
        }

        // //Instantiate all the managers
        // for (int i = 0; i < managerSystems.Count; i++)
        // {
        //     Instantiate(managerSystems[i]);
        // }

        //Instantiate managers
        // poolManager = Instantiate(poolManager);
        // inputManager = Instantiate(inputManager);
        // levelManager = GetComponent<LevelManager>();

    }

    private void Start()
    {
       // Debug.Log("Yo");
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        //DontDestroyOnLoad(this.gameObject);

    }

}
