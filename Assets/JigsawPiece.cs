using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawPiece : MonoBehaviour {
    private bool active = false;
    public int X, Y;

    private void Update()
    {
        if (active)
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.parent.position = new Vector3(position.x, position.y, -1f);
        }
    }

    private void OnMouseDown()
    {
        active = true;
        transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, -1f);
        Debug.Log(string.Format("Piece {0}, {1} is now active", X, Y));
    }

    private void OnMouseUp()
    {
        active = false;
        transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, 0f);
        Debug.Log(string.Format("Piece {0}, {1} is no longer active", X, Y));
    }
}
