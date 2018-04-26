using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaskScript : MonoBehaviour {
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
    public Texture2D MaskImage;
    public int X = 0;
    public int Y = 0;
    public int Nx = 2;
    public int Ny = 2;
    public Edge[] Edges; // Top, Right, Bottom, Left
    private TEdges edges;

    // 184 x 184
    // 33 outwards
    // 

    // Use this for initialization
    void Start ()
    {
        edges = new TEdges(Edges[0], Edges[1], Edges[2], Edges[3]);

        Debug.Log(string.Format("Creating mask for piece {0},{1}", X, Y));

        float w = (float)Image.width / Nx;
        float h = (float)Image.height / Ny;
        var y = Y * h;
        var x = X * w;

        int vY = 6 + (edges.Top == Edge.Out ? 1 : 0) + (edges.Bottom == Edge.Out ? 1 : 0);
        int vX = 6 + (edges.Left == Edge.Out ? 1 : 0) + (edges.Right == Edge.Out ? 1 : 0);
        float hScale = vY / 6f;
        float wScale = vX / 6f;
        w = w * vX / 6f;
        h = h * vY / 6f;

        if (edges.Bottom == Edge.Out)
        {
            y = y - h / vY;
        }

        if (edges.Left == Edge.Out)
        {
            x = x - w / vX;
        }
        
        var texture = new Texture2D(Mathf.RoundToInt(w), Mathf.RoundToInt(h), TextureFormat.ARGB32, false);
        texture.SetPixels(Image.GetPixels(Mathf.RoundToInt(x), Mathf.RoundToInt(y), Mathf.RoundToInt(w), Mathf.RoundToInt(h)));
        texture.Apply();

        var renderer = GetComponent<Renderer>();
        var newMaterial = new Material(renderer.sharedMaterial);
        newMaterial.SetTexture("_BackTex", texture);
        newMaterial.SetTexture("_MaskTex", MaskImage);

        renderer.sharedMaterial = newMaterial;

        var offsetX = (Nx % 2 == 0 ? 0.5f : 0.0f) - X * (1 / 6f);
        var offsetY = (Ny % 2 == 0 ? 0.5f : 0.0f) - Y * (1 / 6f);

        transform.localPosition = new Vector3(X - Nx / 2f + offsetX, Y - Ny / 2f + offsetY);
        //transform.localScale = new Vector3(vX / 4f, vY / 4f);
    }
}
