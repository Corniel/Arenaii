using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves;

public static class Move
{
    public static readonly PassMove Pass;
    public static readonly NoMove None;

    public static IMove Parse(string str, Cells cells)
    {
        var move = (str ?? "").ToUpperInvariant();
        if(string.IsNullOrEmpty(move))
        {
            return None;
        }
        if(move == "PASS")
        {
            return Pass;
        }

        if (move.StartsWith("KILL "))
        {
            var split = move.Substring(5).Split(',');

            if (split.Length == 2 &&
                int.TryParse(split[0], out int x) &&
                x >= 0 && x < cells.Width &&
                int.TryParse(split[1], out int y) &&
                y >= 0 && y < cells.Height)
            {
                var cell = cells[x, y];
                return new KillMove(cell);
            }
        }

        if (move.StartsWith("BIRTH "))
        {
            var parts = move.Substring(6).Split(' ');

            if (parts.Length == 3)
            {
                var child = parts[0].Split(',');
                var father = parts[1].Split(',');
                var mother = parts[2].Split(',');

                if (child.Length == 2 &&
                    father.Length == 2 &&
                    mother.Length == 2 &&

                    int.TryParse(child[0], out int child_x) &&
                    child_x >= 0 && child_x < cells.Width &&
                    int.TryParse(child[1], out int child_y) &&
                    child_y >= 0 && child_y < cells.Height &&

                    int.TryParse(father[0], out int father_x) &&
                    father_x >= 0 && father_x < cells.Width &&
                    int.TryParse(father[1], out int father_y) &&
                    father_y >= 0 && father_y < cells.Height &&

                    int.TryParse(mother[0], out int mother_x) &&
                    mother_x >= 0 && mother_x < cells.Width &&
                    int.TryParse(mother[1], out int mother_y) &&
                    mother_y >= 0 && mother_y < cells.Height)
                {
                    return new BirthMove
                    (
                        cells[child_x, child_y],
                        cells[father_x, father_y],
                        cells[mother_x, mother_y]
                    );
                }
            }
        }
        return None;
    }
}
