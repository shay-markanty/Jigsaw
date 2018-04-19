using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mask : MonoBehaviour {
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
    public Edge[] Edges; // Top, Right, Bottom, Down

    // 184 x 184
    // 33 outwards
    // 

    // Use this for initialization
    void Start ()
    {
        var width = Image.width / Nx;
        var height = Image.height / Ny;

        

        var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        texture.SetPixels(Image.GetPixels(X * width, Y * height, width, height));
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
