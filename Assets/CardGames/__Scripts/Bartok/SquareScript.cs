using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum SquareState
    { button, idle, moving, connected}


[RequireComponent (typeof(BoxCollider2D))]
public class SquareScript : MonoBehaviour
{
    Player_Squares player;
    GameObject endpoint;

    Utils util;
   public SquareState state = SquareState.button;
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    BoxCollider2D collider;
    Color color;
    Transform endLocation;
    Vector2 direction;
    bool directionAssigned = false;
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart, timeDuration;
    [SerializeField]


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player_Squares>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == SquareState.moving)
        {
            
        }
    }

    public void OnMouseUpAsButton()
    {
        if (state == SquareState.button && !directionAssigned) { 
            directionAssigned = true;
            float tempDirection = AssignDirection();
            print(tempDirection);
           direction = parseDirection(tempDirection);
            MoveTo(endpoint.transform.position);
        }
        
        print(name); // When clicked, this outputs the card name
    }


    int AssignDirection(){ return Random.Range(1, 4); }

    Vector2 parseDirection(float directionFloat) {
        //top right
        if (directionFloat == 1)
        {
            endpoint = player.TopRight;
            return new Vector2(1, 1);
        }
        //bottom right
        else if (directionFloat == 2)
        {
            endpoint = player.BottomRight;
            return new Vector2(1, -1);
        }
        //bottom left
        else if (directionFloat == 3)
        {
            endpoint = player.BottomLeft;
            return new Vector2(-1, -1);
        }
        //top left
        else if (directionFloat == 4)
        {
            endpoint = player.TopLeft;
            return new Vector2(-1, 1);
        }
        else
        {
            print("no direction chosen");
            return new Vector2(0, 0);
        }
    }

    // When the card is done moving, it will call reportFinishTo.SendMessage()
    public GameObject reportFinishTo = null;

    [System.NonSerialized]
    public Player callbackPlayer = null;

    // MoveTo tells the card to interpolate to a new position and rotation
    public void MoveTo(Vector3 ePos, Quaternion eRot)
    {
        // Make new interpolation lists for the card.
        // Position and Rotation will each have only two points.
        bezierPts = new List<Vector3>();
        bezierPts.Add(transform.localPosition);  // Current position
        bezierPts.Add(ePos);                     // Current rotation

        bezierRots = new List<Quaternion>();
        bezierRots.Add(transform.rotation);      // New position
        bezierRots.Add(eRot);                    // New rotation

        if (timeStart == 0)
        {
            timeStart = Time.time;
        }

        // timeDuration always starts the same but can be overwritten later
        timeDuration = MOVE_DURATION;
        state = SquareState.moving;
    }

    public void MoveTo(Vector3 ePos)
    {                                
        MoveTo(ePos, Quaternion.identity);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Square")
            state = SquareState.idle;
    }
}