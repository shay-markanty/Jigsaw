﻿using System.Collections;
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
            transform.position = new Vector3(position.x, position.y, -1f);
        }
    }

    private void OnMouseDown()
    {
        active = true;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1f);
        Debug.Log(string.Format("Piece {0}, {1} is now active", X, Y));
    }

    private void OnMouseUp()
    {
        active = false;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        Debug.Log(string.Format("Piece {0}, {1} is no longer active", X, Y));
    }
}