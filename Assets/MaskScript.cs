using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaskScript : MonoBehaviour {
    public Texture2D Image;
    public Texture2D Mask;

    public enum Edge
    {
        Flat,
        In,
        Out
    }

    public class TEdges
    {
        public TEdges(Edge top, Edge right, Edge bottom, Edge left)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Edge Top { get; private set; }
        public Edge Bottom { get; private set; }
        public Edge Left { get; private set; }
        public Edge Right { get; private set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (TEdges)obj;
            return Top == other.Top && Right == other.Right && Bottom == other.Bottom && Left == other.Left;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return (int)Top + 10 * (int)Right + 100 * (int)Left + 1000 * (int)Bottom;
        }
    }

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
