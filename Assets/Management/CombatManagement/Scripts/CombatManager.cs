using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    // Start is called before the first frame update
    public GameObject parryBox;

    public HitStopData reflectHitStop;
    public HitStopData parryHitStop;

    public int parryWindow = 5/ 60;

    private void Awake() {
          if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }
}
