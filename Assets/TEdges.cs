using Model;

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

    internal static TEdges CreateEdges(JigsawPuzzle puzzle, int x, int y)
    {
        Edge top, right, bottom, left;

        var piece = puzzle.Pieces[x, y];
        if (piece.Left == null)
        {
            left = Edge.Flat;
        }
        else
        {
            left = piece.LeftD == Connection.In ? Edge.In : Edge.Out;
        }

        if (piece.Right == null)
        {
            right = Edge.Flat;
        }
        else
        {
            right = piece.RightD == Connection.In ? Edge.In : Edge.Out;
        }

        if (piece.Top == null)
        {
            top = Edge.Flat;
        }
        else
        {
            top = piece.TopD == Connection.In ? Edge.In : Edge.Out;
        }

        if (piece.Bottom == null)
        {
            bottom = Edge.Flat;
        }
        else
        {
            bottom = piece.BottomD == Connection.In ? Edge.In : Edge.Out;
        }

        return new TEdges(top, right, bottom, left);
    }
}
