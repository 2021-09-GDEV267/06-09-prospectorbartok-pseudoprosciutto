using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The SlotDef class is not a subclass of MonoBehaviour, so it doesn't need
//   a separate C# file.

[System.Serializable] // This makes SlotDefs visible in the Unity Inspector pane
public class SlotDef
{
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>();
    public string type = "slot";
    public Vector2 stagger;
}

