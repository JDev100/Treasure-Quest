using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMarker : MonoBehaviour
{
    Interactable interactable;

    public GameObject GetGameObject()
    {
        return interactable.gameObject;
    }

    public Vector2 GetLocation()
    {
        return interactable.location;
    }

    public void SetInteractable(GameObject gameObject, Vector2 location)
    {
        interactable = new Interactable(gameObject, location);
    }

    public Interactable GetInteractable (){
        return interactable;
    }
  
}
