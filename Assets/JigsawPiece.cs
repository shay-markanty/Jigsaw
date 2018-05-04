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

    private void Update()
    {
        if (active)
        {

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            transform.parent.position = ray.GetPoint(dragDistance) + offset - Vector3.forward * 4f;

            //var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.parent.position = new Vector3(position.x - offset.x, position.y - offset.y, -4f);
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
        //    var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //offset = GetComponent<Collider>().ClosestPoint(position);
        //active = true;
        //transform.parent.localPosition = new Vector3(transform.parent.localPosition.x, transform.parent.localPosition.y, -1f);
        
    }

    private void OnMouseUp()
    {
        float z = 0f;
        if (activeColliders.Any())
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

        //if (!active) return;
        Debug.Log("collide with " + other.gameObject.name);
        activeColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!active) return;
        Debug.Log("no more collide with " + other.gameObject.name);
        activeColliders.Remove(other);
    }
}
