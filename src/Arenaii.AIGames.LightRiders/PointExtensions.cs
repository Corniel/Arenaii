using System.Collections.Generic;
using System.Drawing;

namespace Arenaii.AIGames.LightRiders
{
    internal static class PointExtensions
    {
        public static Point Move(this Point p, Move move)
        {
            switch (move)
            {
                case LightRiders.Move.up: /*   */ return new Point(p.X + 0, p.Y - 1);
                case LightRiders.Move.right: /**/ return new Point(p.X + 1, p.Y + 0);
                case LightRiders.Move.down: /* */ return new Point(p.X + 0, p.Y + 1);
                case LightRiders.Move.left: /* */ return new Point(p.X - 1, p.Y + 0);
                default: return p;
            }
        }

        public static bool IsOnBoard(this Point p)
        {
            return
                p.X >= 0 && p.X < 16 &&
                p.Y >= 0 && p.Y < 16;
        }

        public static IEnumerable<Point> GetNeighbors(this Point p)
        {
            var l = new Point(p.X - 1, p.Y + 0);
            var r = new Point(p.X + 1, p.Y + 0);
            var d = new Point(p.X + 0, p.Y + 1);
            var u = new Point(p.X + 0, p.Y - 1);

            if (l.IsOnBoard()) { yield return l; }
            if (r.IsOnBoard()) { yield return r; }
            if (d.IsOnBoard()) { yield return d; }
            if (u.IsOnBoard()) { yield return u; }
        }
    }
}
