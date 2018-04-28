using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGenerator : MonoBehaviour {
    public Transform Prefab;
    public Camera Camera;
    
    public Texture2D[] EdgeMasks;
    // Top: Flat, In, Out
    // Right: Flat, In, Out
    // Bottom: Flat, In, Out
    // Left: Flat, In, Out
    // Center

	// Use this for initialization
	public void Generate(int Nx, int Ny, Texture2D Image) {
        float pieceWidth = (float)Image.width / Nx;
        float pieceHeight = (float)Image.height / Ny;
        float maskWidth = pieceWidth * 8f / 6f;
        float maskHeight = pieceHeight * 8f / 6f;
        var offsetX = pieceWidth / 8f;
        var offsetY = pieceHeight / 8f;
        var totalWidth = Image.width + 2 * offsetX;
        var totalHeight = Image.height + 2 * offsetY;

        // copy image into a bigger texture with borders sized 1 / 8f of the jigsaw image size
        var textureWithBorders = new Texture2D(Mathf.RoundToInt(totalWidth), Mathf.RoundToInt(totalHeight), TextureFormat.ARGB32, false);
        textureWithBorders.SetPixels(Mathf.RoundToInt(offsetX), Mathf.RoundToInt(offsetY), Image.width, Image.height, Image.GetPixels());
        
        for (int x = 0; x < Nx; x++)
        {
            for (int y = 0; y < Ny; y++)
            {
                Vector2 xy = new Vector2(x * pieceWidth + offsetX, y * pieceHeight + offsetY);
                Vector2 xyt = new Vector2(xy.x - pieceWidth / 8f, xy.y - pieceHeight / 8f);
                
                var w = Mathf.Min(maskWidth, textureWithBorders.width - xyt.x);
                var h = Mathf.Min(maskHeight, textureWithBorders.height - xyt.y);
                var tex = new Texture2D(Mathf.RoundToInt(w), Mathf.RoundToInt(h));

                tex.SetPixels(textureWithBorders.GetPixels(Mathf.RoundToInt(xyt.x), Mathf.RoundToInt(xyt.y), Mathf.RoundToInt(w), Mathf.RoundToInt(h)));
                tex.Apply();

                Debug.Log(string.Format("Creating piece {0},{1}", x, y));
                // var tex = 
                var piece = Instantiate(Prefab, transform);
                var mask = piece.GetComponent<MaskScript>();
                mask.Image = tex;
                mask.Edges = CreateEdges(x, y, Nx, Ny);
                mask.EdgeMasks = EdgeMasks;
                
                var tX = (Nx % 2 == 0 ? 0.5f : 0.0f) - x * (2 / 8f);
                var tY= (Ny % 2 == 0 ? 0.5f : 0.0f) - y * (2 / 8f);

                mask.transform.localPosition = new Vector3(x- Nx / 2f + tX, y - Ny / 2f + tY);
            }
        }

        Camera.orthographicSize = Mathf.Max(Nx, Ny) / 2f;
        // This will keep the aspect ratio of the object the same as the image's
        // the original object's (before the transform) is Nx / Ny
        // heuristically transform the smaller of the two edges (x or y) and keep the other at the same size
        // TODO: might need an overall scale up or down to fill screen afterwards
        if (Nx > Ny)
        {
            transform.localScale = new Vector3(1f, (float)Image.height / Image.width * ((float)Nx / Ny));
        }
        else
        {
            transform.localScale = new Vector3(1f / ((float)Image.height / Image.width * ((float)Nx / Ny)), 1f);
        }
	}
    
    MaskScript.Edge[] CreateEdges(int x, int y, int Nx, int Ny)
    {
        MaskScript.Edge top, right, bottom, left;

        if (x == 0)
        {
            left = MaskScript.Edge.Flat;
            right = MaskScript.Edge.Out;
        }
        else if (x == Nx - 1) 
        {
            left = MaskScript.Edge.In;
            right = MaskScript.Edge.Flat;
        }
        else
        {
            left = MaskScript.Edge.In;
            right = MaskScript.Edge.Out;
        }

        if (y == 0)
        {
            bottom = MaskScript.Edge.Flat;
            top = MaskScript.Edge.Out;
        }
        else if (y == Ny - 1)
        {
            bottom = MaskScript.Edge.In;
            top = MaskScript.Edge.Flat;
        }
        else
        {
            bottom = MaskScript.Edge.In;
            top = MaskScript.Edge.Out;
        }

        return new[] { top, right, bottom, left };
    }
}
