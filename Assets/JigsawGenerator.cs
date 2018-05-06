using Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JigsawGenerator : MonoBehaviour
{
    // Top: Flat, In, Out
    // Right: Flat, In, Out
    // Bottom: Flat, In, Out
    // Left: Flat, In, Out
    // Center
    private Texture2D[] edgeMasks;

    public Transform Prefab;
    public Camera Camera;
    public Texture2D Image;
    public Texture2D[] EdgeTemplates; //BottomFlat,  BottomIn, BottomOut, Center
    public Dictionary<TEdges, Texture2D> CachedMasks;
    public bool SetRandomLocations = false;

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

        Generate(4, 4, Image);
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
    
    public void Generate(int Nx, int Ny, Texture2D Image)
    {
        CachedMasks = new Dictionary<TEdges, Texture2D>();

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

        int n = Nx * Ny;
        int[] array = new int[n];
        for (int i = 0; i < n; i++)
        {
            array[i] = i;
        }
        Shuffle(array);

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

                var pieceContainer = Instantiate(Prefab, transform);
                var piece = pieceContainer.Find("JigsawPiece").GetComponent<JigsawPiece>();
                
                var mask = piece.GetComponent<MaskScript>();
                mask.Image = tex;

                var edges = TEdges.CreateEdges(puzzle, x, y);
                piece.Instantiate(x, y, edges);

                CreateMaskTexture(edges);
                mask.Mask = CachedMasks[edges];

                var tX = (Nx % 2 == 0 ? 0.5f : 0.0f); // - x * (2 / 8f);
                var tY = (Ny % 2 == 0 ? 0.5f : 0.0f); // - y * (2 / 8f);

                var pX = x - Nx / 2f + tX;
                var pY = y - Ny / 2f + tY;

                if (SetRandomLocations)
                {
                    var r = array[x * Ny + y];

                    pX = r % Ny - Nx / 2f + tX;
                    pY = r / Ny - Ny / 2f + tY;

                    // Randomize location
                    // pX =  UnityEngine.Random.Range(-Nx / 2.5f, Nx / 2.5f);
                    // pY = UnityEngine.Random.Range(-Ny / 2.5f, Ny / 2.5f);
                }

                pieceContainer.transform.localPosition = new Vector3(pX, pY);
                Camera.transform.position = new Vector3(tX, tY, -10f);
            }
        }

        Camera.orthographicSize = (Mathf.Max(Nx, Ny) / 2f) * 3f / 2f;
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

    /// <summary>
    /// Knuth shuffle
    /// </summary>        
    public void Shuffle(int[] array)
    {
        System.Random random = new System.Random();
        int n = array.Count();
        while (n > 1)
        {
            n--;
            int i = random.Next(n + 1);
            int temp = array[i];
            array[i] = array[n];
            array[n] = temp;
        }
    }

    private void CreateMaskTexture(TEdges edges)
    {
        if (!CachedMasks.ContainsKey(edges))
        {
            var topTex = edgeMasks[edges.Top == Edge.Flat ? 0 : edges.Top == Edge.In ? 1 : 2];
            var rightTex = edgeMasks[edges.Right == Edge.Flat ? 3 : edges.Right == Edge.In ? 4 : 5];
            var bottomTex = edgeMasks[edges.Bottom == Edge.Flat ? 6 : edges.Bottom == Edge.In ? 7 : 8];
            var leftTex = edgeMasks[edges.Left == Edge.Flat ? 9 : edges.Left == Edge.In ? 10 : 11];

            var newMask = new Texture2D(edgeMasks[0].width, edgeMasks[1].height);
            for (int i = 0; i < edgeMasks[0].width; ++i)
            {
                for (int j = 0; j < edgeMasks[0].height; ++j)
                {
                    var col = edgeMasks[12].GetPixel(i, j) + topTex.GetPixel(i, j) + rightTex.GetPixel(i, j) + bottomTex.GetPixel(i, j) + leftTex.GetPixel(i, j);
                    newMask.SetPixel(i, j, col);
                }
            }

            newMask.wrapMode = TextureWrapMode.Clamp;
            newMask.Apply();
            CachedMasks[edges] = newMask;
        }
    }
}
