using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerInput : MonoBehaviour
{
    [Header("Essential Components")]
    Player player;

    bool hasPressedRTrigger;

    /***********************************************/
    /***********************************************/
    /*          START FUNCTION                     */
    /***********************************************/
    /***********************************************/
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //float rTrigger = Input.GetAxisRaw("RightTrigger");
        player.SetDirectionalInput(directionalInput);

        if (Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetButtonDown("Fire1")) {
            player.OnAttackInputDown();
        } 

        if (Input.GetButtonDown("Dash")) {
            player.OnDashInputDown((int)directionalInput.x);
        }

        //if ()
    }

    public void TriggerGameOver() {
        LevelManager.instance.PlayerGameOver();
        EffectsManager.instance.TriggerHitStop();
    }
}
