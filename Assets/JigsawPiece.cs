using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JigsawPiece : MonoBehaviour {
    private bool active = false;
    public int X, Y;
    Vector3 offset;
    private List<Collider> activeColliders = new List<Collider>();
    float dragDistance;
    public TEdges Edges;

    public void Instantiate(int x, int y, TEdges edges)
    {
        X = x;
        Y = y;
        Edges = edges;

        gameObject.name = string.Format("JigsawPiece ({0},{1})", x, y);
        GetComponent<BoxCollider>().size = new Vector3(6f / 8f, 6f / 8f, 5f);
        var upCollider = transform.Find("UpCollider");
        var bottomCollider = transform.Find("BottomCollider");
        var rightCollider = transform.Find("RightCollider");
        var leftCollider = transform.Find("LeftCollider");

        switch (Edges.Top)
        {
            case Edge.In:
                upCollider.localPosition = new Vector3(0f, 0.325f, 0f);
                break;
            case Edge.Flat:
                GameObject.Destroy(upCollider.gameObject);
                break;
        }

        switch (Edges.Bottom)
        {
            case Edge.In:
                bottomCollider.localPosition = new Vector3(0f, -0.325f, 0f);
                break;
            case Edge.Flat:
                GameObject.Destroy(bottomCollider.gameObject);
                break;
        }
        
        switch (Edges.Right)
        {
            case Edge.In:
                rightCollider.localPosition = new Vector3(0.325f, 0f, 0f);
                break;
            case Edge.Flat:
                GameObject.Destroy(rightCollider.gameObject);
                break;
        }

        switch (Edges.Left)
        {
            case Edge.In:
                leftCollider.localPosition = new Vector3(-0.325f, 0f, 0f);
                break;
            case Edge.Flat:
                GameObject.Destroy(leftCollider.gameObject);
                break;
        }
    }

    private void Update()
    {
        if (active)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var position = ray.GetPoint(dragDistance) + offset;
            transform.parent.position = new Vector3(position.x, position.y, -4f); // ray.GetPoint(dragDistance) + offset - Vector3.forward * 4f;
        }
    }

    private void OnMouseDown()
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform == transform)
        {
            Debug.Log(string.Format("Piece {0}, {1} is now active", X, Y));

            active = true;
            dragDistance = hit.distance;
            offset = transform.parent.position - hit.point;
        }
    }

    private void OnMouseUp()
    {
        float z = 0f;
        if (activeColliders.Any(c => c.transform.parent != transform.parent))
        {
            z = activeColliders.Min(c => c.transform.parent.position.z) - 0.01f;            
        }

        active = false;
        transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, z);
        Debug.Log(string.Format("Piece {0}, {1} is no longer active. New position: {2}", X, Y, z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "JigsawPiece") return;
        
        Debug.Log("collide with " + other.gameObject.name);
        activeColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("no more collide with " + other.gameObject.name);
        activeColliders.Remove(other);
    }
}
