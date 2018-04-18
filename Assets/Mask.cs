using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Mask : MonoBehaviour {

    public Texture2D Image;
    public Sprite MaskImage;
    public int X = 0;
    public int Y = 0;
    // Use this for initialization
    void Start ()
    {
        var texture = new Texture2D(25, 25, TextureFormat.ARGB32, false);
        texture.SetPixels(Image.GetPixels(X * 25, Y * 25, 25, 25));
        texture.Apply();

        var renderer = GetComponent<Renderer>();
        var newMaterial = new Material(renderer.sharedMaterial);
        newMaterial.SetTexture("_BackTex", texture);
        newMaterial.SetTexture("_MaskTex", MaskImage.texture);

        renderer.sharedMaterial = newMaterial;
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
