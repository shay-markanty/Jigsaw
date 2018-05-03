using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigsawGenerator : MonoBehaviour
{
    private Texture2D[] edgeMasks;

    public Transform Prefab;
    public Camera Camera;
    public Texture2D Image;
    public Texture2D[] EdgeTemplates; //BottomFlat,  BottomIn, BottomOut, Center
    // Top: Flat, In, Out
    // Right: Flat, In, Out
    // Bottom: Flat, In, Out
    // Left: Flat, In, Out
    // Center

    private void Start()
    {
        edgeMasks = new Texture2D[13];
        edgeMasks[0] = FlipTextureVertically(EdgeTemplates[0]);
        edgeMasks[1] = FlipTextureVertically(EdgeTemplates[1]);
        edgeMasks[2] = FlipTextureVertically(EdgeTemplates[2]);
        edgeMasks[3] = RotateTextureClockwise(edgeMasks[0]);
        edgeMasks[4] = RotateTextureClockwise(edgeMasks[1]);
        edgeMasks[5] = RotateTextureClockwise(edgeMasks[2]);
        edgeMasks[6] = EdgeTemplates[0];
        edgeMasks[7] = EdgeTemplates[1];
        edgeMasks[8] = EdgeTemplates[2];
        edgeMasks[9] = RotateTextureClockwise(edgeMasks[6]);
        edgeMasks[10] = RotateTextureClockwise(edgeMasks[7]);
        edgeMasks[11] = RotateTextureClockwise(edgeMasks[8]);
        edgeMasks[12] = EdgeTemplates[3];

        Generate(5, 5, Image);
    }

    private Texture2D FlipTextureVertically(Texture2D texture2D)
    {
        Texture2D result = new Texture2D(texture2D.width, texture2D.height);
        result.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < texture2D.width; ++i)
        {
            for (int j = 0; j < texture2D.height; ++j)
            {
                result.SetPixel(i, texture2D.height - j - 1, texture2D.GetPixel(i, j));
            }
        }

        result.Apply();
        return result;
    }

    private Texture2D RotateTextureClockwise(Texture2D texture2D)
    {
        Texture2D result = new Texture2D(texture2D.height, texture2D.width);
        result.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < texture2D.width; ++i)
        {
            for (int j = 0; j < texture2D.height; ++j)
            {
                result.SetPixel(j, i, texture2D.GetPixel(i, j));
            }
        }

        result.Apply();
        return result;
    }

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
        JigsawPuzzle puzzle = JigsawPuzzle.Generate(Nx, Ny);

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
                mask.Edges = CreateEdges(puzzle, x, y, Nx, Ny);
                mask.EdgeMasks = edgeMasks;

                var tX = (Nx % 2 == 0 ? 0.5f : 0.0f) - x * (2 / 8f);
                var tY= (Ny % 2 == 0 ? 0.5f : 0.0f) - y * (2 / 8f);

                mask.transform.localPosition = new Vector3(x- Nx / 2f + tX, y - Ny / 2f + tY);

                var jigsawPiece = piece.GetComponent<JigsawPiece>();
                jigsawPiece.X = x;
                jigsawPiece.Y = y;
            }
        }

        Camera.orthographicSize = Mathf.Max(Nx, Ny) / 2f;
        // This will keep the aspect ratio of the object the same as the image's
        // the original object's (before the transform) is Nx / Ny
        // heuristically transform the smaller of the two edges (x or y) and keep the other at the same size
        // TODO: might need an overall scale up or down to fill screen afterwards
        if (Nx > Ny)
        {
            transform.localScale = new Vector3(1f, (float)Image.height / Image.width * ((float)Nx / Ny), 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f / ((float)Image.height / Image.width * ((float)Nx / Ny)), 1f, 1f);
        }
	}
    
    MaskScript.Edge[] CreateEdges(JigsawPuzzle puzzle, int x, int y, int Nx, int Ny)
    {
        MaskScript.Edge top, right, bottom, left;

        var piece = puzzle.Pieces[x, y];
        if (piece.Left == null)
        {
            left = MaskScript.Edge.Flat;
        }
        else
        {
            left = piece.LeftD == Connection.In ? MaskScript.Edge.In : MaskScript.Edge.Out;
        }

        if (piece.Right == null)
        {
            right = MaskScript.Edge.Flat;
        }
        else
        {
            right = piece.RightD == Connection.In ? MaskScript.Edge.In : MaskScript.Edge.Out;
        }

        if (piece.Top == null)
        {
            top = MaskScript.Edge.Flat;
        }
        else
        {
            top = piece.TopD == Connection.In ? MaskScript.Edge.In : MaskScript.Edge.Out;
        }

        if (piece.Bottom == null)
        {
            bottom = MaskScript.Edge.Flat;
        }
        else
        {
            bottom = piece.BottomD == Connection.In ? MaskScript.Edge.In : MaskScript.Edge.Out;
        }

        //if (x == 0)
        //{
        //    left = MaskScript.Edge.Flat;
        //    right = MaskScript.Edge.Out;
        //}
        //else if (x == Nx - 1) 
        //{
        //    left = MaskScript.Edge.In;
        //    right = MaskScript.Edge.Flat;
        //}
        //else
        //{
        //    left = MaskScript.Edge.In;
        //    right = MaskScript.Edge.Out;
        //}

        //if (y == 0)
        //{
        //    bottom = MaskScript.Edge.Flat;
        //    top = MaskScript.Edge.Out;
        //}
        //else if (y == Ny - 1)
        //{
        //    bottom = MaskScript.Edge.In;
        //    top = MaskScript.Edge.Flat;
        //}
        //else
        //{
        //    bottom = MaskScript.Edge.In;
        //    top = MaskScript.Edge.Out;
        //}

        return new[] { top, right, bottom, left };
    }
}
