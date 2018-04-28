using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaskScript : MonoBehaviour {
    private TEdges edges;

    public enum Edge
    {
        Flat,
        In,
        Out
    }

    class TEdges
    {
        public TEdges(Edge top, Edge right, Edge bottom, Edge left)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Edge Top { get; private set; }
        public Edge Bottom { get; private set; }
        public Edge Left { get; private set; }
        public Edge Right { get; private set; }
    }

    public Texture2D Image;
    public Texture2D[] EdgeMasks;
    public Edge[] Edges; // Top, Right, Bottom, Left
    
    // Use this for initialization
    void Start ()
    {
        edges = new TEdges(Edges[0], Edges[1], Edges[2], Edges[3]);

        var renderer = GetComponent<Renderer>();
        var newMaterial = new Material(renderer.sharedMaterial);
        newMaterial.SetTexture("_BackTex", Image); //texture);
        newMaterial.SetTexture("_CenterTex", EdgeMasks[12]);
        newMaterial.SetTexture("_TopTex", EdgeMasks[edges.Top == Edge.Flat ? 0 : edges.Top == Edge.In ? 1 : 2]);
        newMaterial.SetTexture("_RightTex", EdgeMasks[edges.Right == Edge.Flat ? 3 : edges.Right == Edge.In ? 4 : 5]);
        newMaterial.SetTexture("_BottomTex", EdgeMasks[edges.Bottom == Edge.Flat ? 6 : edges.Bottom == Edge.In ? 7 : 8]);
        newMaterial.SetTexture("_LeftTex", EdgeMasks[edges.Left == Edge.Flat ? 9 : edges.Left == Edge.In ? 10 : 11]);

        renderer.sharedMaterial = newMaterial;
    }
}
