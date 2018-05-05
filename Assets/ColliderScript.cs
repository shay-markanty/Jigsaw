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

    private GameObject group;
    private JigsawPiece piece;

    private void Start()
    {
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "BottomCollider" && 
            other.tag != "LeftCollider" && 
            other.tag != "UpCollider" && 
            other.tag != "RightCollider")
            return;

        group = transform.parent.parent.gameObject;
        piece = transform.GetComponentInParent<JigsawPiece>();

        JigsawPiece otherPiece = other.transform.GetComponentInParent<JigsawPiece>();

        switch (Direction)
        {
            case ColliderType.Up:
                if (other.tag == "BottomCollider")
                {
                    Debug.Log("Found bottom collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.Y == piece.Y + 1)
                    {
                        Debug.Log(string.Format("Found a connection between {0},{1} and {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                    }
                }
                break;
            case ColliderType.Right:
                if (other.tag == "LeftCollider")
                {
                    Debug.Log("Found left collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.X == piece.X + 1)
                    {
                        Debug.Log(string.Format("Found a connection between {0},{1} and {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                    }
                }
                break;
            case ColliderType.Down:
                if (other.tag == "UpCollider")
                {
                    Debug.Log("Found top collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.Y == piece.Y - 1)
                    {
                        Debug.Log(string.Format("Found a connection between {0},{1} and {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                    }
                }
                break;
            case ColliderType.Left:
                if (other.tag == "RightCollider")
                {
                    Debug.Log("Found right collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.X == piece.X - 1)
                    {
                        Debug.Log(string.Format("Found a connection between {0},{1} and {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                    }
                }
                break;
            default:
                break;
        }
    }
}
