using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***********************************************/
/***********************************************/
/*              LEVEL EDITING SAVE SYSTEM      */
/***********************************************/
/***********************************************/

[System.Serializable]
public class LevelData
{
    public string[] interactables;

    public LevelData(List<string> interactableSetUp)
    {
        interactables = new string[interactableSetUp.Count];
        for (int i = 0; i < interactableSetUp.Count; i++)
        {
            interactables[i] = interactableSetUp[i];
        }
    }
}