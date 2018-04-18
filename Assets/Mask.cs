using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mask : MonoBehaviour {

    public Texture2D Image;
    public Texture2D MaskImage;
    public int X = 0;
    public int Y = 0;
    public int Nx = 2;
    public int Ny = 2;
    // Use this for initialization
    void Start ()
    {
        var w = Image.width / Nx;
        var h = Image.height / Ny;
        var texture = new Texture2D(w, h, TextureFormat.ARGB32, false);
        texture.SetPixels(Image.GetPixels(X * w, Y * h, w, h));
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
