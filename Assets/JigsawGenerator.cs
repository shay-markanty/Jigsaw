using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGenerator : MonoBehaviour {
    public Transform Prefab;
    public Texture2D Image, Mask;
    public int Nx, Ny;
    public Camera Camera;
    public Texture2D[] Masks;
    // Top-left, Top, Top-Right, Right, Bottom-Right, Bottom, Bottom-Left, Left, Middle

    public Texture2D[] EdgeMasks;
    // Top: Flat, In, Out
    // Right: Flat, In, Out
    // Bottom: Flat, In, Out
    // Left: Flat, In, Out
    // Center

	// Use this for initialization
	void Start () {
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
                mask.Edges = CreateEdges(x, y);
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

    Texture2D ChooseMask(int x, int y)
    {
        // 0 = Top-left, 1 = Top, 2 = Top-Right, 3 = Right, 4 = Bottom-Right, 5 = Bottom, 6 = Bottom-Left, 7 = Left, 8 = Middle
        if (x == 0 && y == Ny - 1)
        {
            return Masks[0];
        }
        else if (x == Nx - 1 && y == Ny - 1)
        {
            return Masks[2];
        }
        else if (x == Nx - 1 && y == 0)
        {
            return Masks[4];
        }
        else if (x == 0 && y == 0)
        {
            return Masks[6];
        }
        else if (x == 0)
        {
            return Masks[7];
        }
        else if (x == Nx - 1)
        {
            return Masks[3];
        }
        else if (y == 0)
        {
            return Masks[5];
        }
        else if (y == Ny - 1)
        {
            return Masks[1];
        }
        else return Masks[8];
    }

    MaskScript.Edge[] CreateEdges(int x, int y)
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

        //if (x == 0 && y == 0)
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Out, MaskScript.Edge.Flat, MaskScript.Edge.Flat };
        //}
        //else if (x == 0 && y == Ny - 1)
        //{
        //    return new[] { MaskScript.Edge.Flat, MaskScript.Edge.Out, MaskScript.Edge.In, MaskScript.Edge.Flat };
        //}
        //else if (x == Nx - 1 && y == 0)
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Flat, MaskScript.Edge.Flat, MaskScript.Edge.In };
        //}
        //else if (x == Nx - 1 && y == Ny - 1)
        //{
        //    return new[] { MaskScript.Edge.Flat, MaskScript.Edge.Flat, MaskScript.Edge.In, MaskScript.Edge.In };
        //}
        //else if (x == 0)
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Out, MaskScript.Edge.Flat, MaskScript.Edge.Flat };
        //}
        //else if (x == Nx - 1)
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Flat, MaskScript.Edge.In, MaskScript.Edge.In };
        //}
        //else if (y == 0)
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Out, MaskScript.Edge.Flat, MaskScript.Edge.In };
        //}
        //else if (y == Ny - 1)
        //{
        //    return new[] { MaskScript.Edge.Flat, MaskScript.Edge.Out, MaskScript.Edge.In, MaskScript.Edge.In };
        //}
        //else
        //{
        //    return new[] { MaskScript.Edge.Out, MaskScript.Edge.Out, MaskScript.Edge.In, MaskScript.Edge.In };
        //}
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
