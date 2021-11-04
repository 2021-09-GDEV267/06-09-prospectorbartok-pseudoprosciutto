using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The SlotDef class is not a subclass of MonoBehaviour, so it doesn't need
//   a separate C# file.

[System.Serializable]                                               
public class SlotDef
{                                                
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public List<int> hiddenBy = new List<int>(); // Unused in Bartok
    public float rot;         // rotation of hands
    public string type = "slot";
    public Vector2 stagger;
    public int player;     // player number of a hand
    public Vector3 pos;        // pos derived from x, y, & multiplier
}
