using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGenerator : MonoBehaviour {
    public Transform Prefab;
    public Texture2D Image, Mask;
    public int Nx, Ny;
    public Camera Camera;

	// Use this for initialization
	void Start () {
        for (int x = 0; x < Nx; x++)
        {
            for (int y = 0; y < Ny; y++)
            {
                var piece = Instantiate(Prefab, transform);
                var mask = piece.GetComponent<Mask>();
                mask.X = x;
                mask.Y = y;
                mask.Nx = Nx;
                mask.Ny = Ny;
                mask.MaskImage = Mask;
                mask.Image = Image;
                mask.transform.localPosition = new Vector3(x - Nx / 2 + (Nx % 2 == 0 ? 0.5f : 0.0f), y - Ny / 2 + (Ny % 2 == 0 ? 0.5f : 0.0f));
            }
        }

        Camera.orthographicSize = Mathf.Max(Nx, Ny) / 2f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
