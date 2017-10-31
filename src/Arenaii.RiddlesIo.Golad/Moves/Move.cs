using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves
{
    public static class Move
    {
        public static readonly PassMove Pass;
        public static readonly NoMove None;

        public static IMove Parse(string str, Cells cells)
        {
            if(string.IsNullOrEmpty(str))
            {
                return None;
            }
            if(str == "pass")
            {
                return Pass;
            }

            if (str.StartsWith("kill "))
            {
                var split = str.Substring(5).Split(',');

                if (split.Length == 2 &&
                    int.TryParse(split[0], out int row) &&
                    row >= 0 && row < cells.Height &&
                    int.TryParse(split[1], out int col) &&
                    col >= 0 && col < cells.Width)
                {
                    var cell = cells[row, col];
                    return new KillMove(cell);
                }
            }

            if (str.StartsWith("birth "))
            {
                var parts = str.Substring(6).Split(' ');

                if (parts.Length == 3)
                {
                    var child = parts[0].Split(',');
                    var father = parts[1].Split(',');
                    var mother = parts[2].Split(',');

                    if (child.Length == 2 &&
                        father.Length == 2 &&
                        mother.Length == 2 &&

                        int.TryParse(child[0], out int child_row) &&
                        child_row >= 0 && child_row < cells.Height &&
                        int.TryParse(child[1], out int child_col) &&
                        child_col >= 0 && child_col < cells.Width &&

                        int.TryParse(father[0], out int father_row) &&
                        father_row >= 0 && father_row < cells.Height &&
                        int.TryParse(father[1], out int father_col) &&
                        father_col >= 0 && father_col < cells.Width &&

                        int.TryParse(mother[0], out int mother_row) &&
                        mother_row >= 0 && mother_row < cells.Height &&
                        int.TryParse(mother[1], out int mother_col) &&
                        mother_col >= 0 && mother_col < cells.Width)
                    {
                        return new BirthMove
                        (
                            cells[child_row, child_col],
                            cells[father_row, father_col],
                            cells[mother_row, mother_col]
                        );
                    }
                }
            }
            return None;
        }
    }
}
