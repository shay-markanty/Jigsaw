using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour {
    private Collider other;

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
        if (other.tag != "BottomCollider" &&
            other.tag != "LeftCollider" &&
            other.tag != "UpCollider" &&
            other.tag != "RightCollider")
            return;

        this.other = other;
        //var piece = transform.GetComponentInParent<JigsawPiece>();
        //JigsawPiece otherPiece = other.transform.GetComponentInParent<JigsawPiece>();
        //Debug.Log(string.Format("Connections overlap between {0}, {1} with {2}, {3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));

        GenerateConnection();
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    var piece = transform.GetComponentInParent<JigsawPiece>();
    //    JigsawPiece otherPiece = other.transform.GetComponentInParent<JigsawPiece>();
    //    Debug.Log(string.Format("No more connections overlap between {0}, {1} with {2}, {3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
    //    this.other = null;    
    //}

    //private void Update()
    //{
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        Debug.Log("Identified mouse up for object " + transform.parent.gameObject.name);
    //        if (this.other != null)
    //        {
    //            GenerateConnection();
    //        }
    //    }
    //}

    private void GenerateConnection()
    { 
        var group = transform.parent.parent.gameObject;
        var piece = transform.GetComponentInParent<JigsawPiece>();
        Vector3 offset = Vector3.zero;
        JigsawPiece otherPiece = other.transform.GetComponentInParent<JigsawPiece>();


        if (piece.transform.parent == otherPiece.transform.parent)
            return;
        
        ColliderType? connect = null;
        switch (Direction)
        {
            case ColliderType.Up:
                if (other.tag == "BottomCollider")
                {
                    Debug.Log("Found bottom collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.X == piece.X && otherPiece.Y == piece.Y + 1)
                    {
                        connect = ColliderType.Up;
                        offset = new Vector3(0f, (6f / 8f) * piece.transform.lossyScale.y, 0f);
                        Debug.Log(string.Format("Found a connection between Up: {0},{1} to Bottom: {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                    }
                }
                break;
            case ColliderType.Right:
                if (other.tag == "LeftCollider")
                {
                    Debug.Log("Found left collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.Y == piece.Y && otherPiece.X == piece.X + 1)
                    {
                        connect = ColliderType.Right;
                        Debug.Log(string.Format("Found a connection between Right: {0},{1} to Left: {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));

                        offset = new Vector3((6f / 8f) * piece.transform.lossyScale.x, 0f, 0f);
                    }
                }
                break;
            case ColliderType.Down:
                if (other.tag == "UpCollider")
                {
                    Debug.Log("Found top collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.X == piece.X && otherPiece.Y == piece.Y - 1)
                    {
                        connect = ColliderType.Down;
                        Debug.Log(string.Format("Found a connection between Bottom: {0},{1} to Up: {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                        offset = new Vector3(0f, -(6f / 8f) * piece.transform.lossyScale.y, 0f);
                    }
                }
                break;
            case ColliderType.Left:
                if (other.tag == "RightCollider")
                {
                    Debug.Log("Found right collider of item " + other.transform.parent.gameObject.name);
                    if (otherPiece.Y == piece.Y &&  otherPiece.X == piece.X - 1)
                    {
                        connect = ColliderType.Left;
                        Debug.Log(string.Format("Found a connection between Left: {0},{1} to Right: {2},{3}", piece.X, piece.Y, otherPiece.X, otherPiece.Y));
                        offset = new Vector3(-(6f / 8f) * piece.transform.lossyScale.x, 0f, 0f);
                    }
                }
                break;
        }

        if (connect != null)
        {
            var translateVector = (piece.transform.position + offset - otherPiece.transform.position);
            var oldParent = otherPiece.transform.parent;

            while (oldParent.childCount > 0)
            {
                var t = oldParent.GetChild(0);
                t.SetParent(piece.transform.parent);
                 t.transform.Translate(translateVector);
            }

            piece.transform.parent.localPosition = new Vector3(
                piece.transform.parent.localPosition.x,
                piece.transform.parent.localPosition.y,
                Mathf.Max(piece.transform.parent.localPosition.z, oldParent.localPosition.z));

            Destroy(other.gameObject);
            Destroy(gameObject);
            Destroy(oldParent.gameObject);
        }
    }
}
