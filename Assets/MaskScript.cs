using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaskScript : MonoBehaviour {
    public Texture2D Image;
    public Texture2D Mask;
    
    // Use this for initialization
    void Start ()
    {
        var renderer = GetComponent<Renderer>();
        var newMaterial = new Material(renderer.sharedMaterial);
        newMaterial.SetTexture("_BackTex", Image);
        newMaterial.SetTexture("_MaskTex", Mask);
        
        renderer.sharedMaterial = newMaterial;
    }
}
