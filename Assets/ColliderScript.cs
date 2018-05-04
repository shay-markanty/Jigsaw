using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour {
    public enum ColliderType
    {
        Up,
        Right,
        Down,
        Left
    }

    public ColliderType Direction;

    private void OnTriggerEnter(Collider other)
    {
        switch (Direction)
        {
            case ColliderType.Up:
                if (other.tag == "BottomCollider")
                {
                    Debug.Log("Found bottom collider of item " + other.transform.parent.gameObject.name);
                }
                break;
            case ColliderType.Right:
                if (other.tag == "LeftCollider")
                {
                    Debug.Log("Found left collider of item " + other.transform.parent.gameObject.name);
                }
                break;
            case ColliderType.Down:
                if (other.tag == "UpCollider")
                {
                    Debug.Log("Found top collider of item " + other.transform.parent.gameObject.name);
                }
                break;
            case ColliderType.Left:
                if (other.tag == "RightCollider")
                {
                    Debug.Log("Found right collider of item " + other.transform.parent.gameObject.name);
                }
                break;
            default:
                break;
        }
    }
}
