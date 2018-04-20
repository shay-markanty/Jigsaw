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

    public Texture2D Image;
    public Texture2D MaskImage;
    public int X = 0;
    public int Y = 0;
    public int Nx = 2;
    public int Ny = 2;
    public Edge[] Edges; // Top, Right, Bottom, Left

    // 184 x 184
    // 33 outwards
    // 

    // Use this for initialization
    void Start ()
    {

        Debug.Log(string.Format("Creating mask for piece {0},{1}", X, Y));

        float width = Image.width / Nx;
        float height = Image.height / Ny;
        var top = Y * height;
        var left = X * width;
        
        float hScale = (4 + (Edges[0] == Edge.Out ? 1 : 0) + (Edges[2] == Edge.Out ? 1 : 0)) / 4f;
        float wScale = (4 + (Edges[1] == Edge.Out ? 1 : 0) + (Edges[3] == Edge.Out ? 1 : 0)) / 4f;
        width *= wScale;
        height *= hScale;

        if (Edges[0] == Edge.Out && Y > 0)
        {
            top -= height / 3;
        }

        if (Edges[3] == Edge.Out)
        {
            left -= width / 3;
        }
        
        var texture = new Texture2D(Mathf.FloorToInt(width), Mathf.FloorToInt(height), TextureFormat.ARGB32, false);
        texture.SetPixels(Image.GetPixels(Mathf.FloorToInt(left), Mathf.FloorToInt(top), Mathf.FloorToInt(width), Mathf.FloorToInt(height)));
        texture.Apply();

        var renderer = GetComponent<Renderer>();
        var newMaterial = new Material(renderer.sharedMaterial);
        newMaterial.SetTexture("_BackTex", texture);
        newMaterial.SetTexture("_MaskTex", MaskImage);

        renderer.sharedMaterial = newMaterial;
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
