namespace Arenaii.AIGames.UltimateTicTacToe;

public class MetaBoard
{
    private readonly Field[] fields = new Field[9 * 9];
    private readonly TinyBoard[] tinies = new TinyBoard[9];

    public MetaBoard()
    {
        LastIndex = -1;

        var index = 0;
        tinies[0] = new TinyBoard();
        tinies[1] = new TinyBoard();
        tinies[2] = new TinyBoard();
        tinies[3] = new TinyBoard();
        tinies[4] = new TinyBoard();
        tinies[5] = new TinyBoard();
        tinies[6] = new TinyBoard();
        tinies[7] = new TinyBoard();
        tinies[8] = new TinyBoard();

        for (var row = 0; row < 9; row++)
        {
            for (var col = 0; col < 9; col++)
            {
                var field = new Field(row, col);
                fields[index++] = field;
                int tiny = ToTiny(row, col);

                var tIndex = 3 * (row % 3) + (col % 3);
                tinies[tiny][tIndex] = field;
            }
        }
    }

    public TinyBoard this[int index]
    {
        get
        {
            return tinies[index];
        }
        set
        {
            tinies[index] = value;
        }
    }

    public int[] MacroBoard
    {
        get
        {
            var macro = new int[9];

            var tiny = 0;
            var all = LastIndex == -1;
            if (!all)
            {
                tiny = ToTinyIndex(LastIndex);
                all = tinies[tiny].State != BoardState.None;
            }

            for (var index = 0; index < 9; index++)
            {
                switch (tinies[index].State)
                {
                    case BoardState.Player1: macro[index] = 1; break;
                    case BoardState.Player2: macro[index] = 2; break;
                    case BoardState.Draw: macro[index] = 0; break;
                    default: macro[index] = all || index == tiny ? -1 : 0; break;
                }
            }
            return macro;
        }
    }
    public string ToMacro() { return string.Join(",", MacroBoard); }

    public void ToConsole()
    {
        Console.ForegroundColor = ConsoleColor.White;

        for (var index = 0; index < 81; index++)
        {
            var tiny = ToTiny(index / 9, index % 9);
            switch (tinies[tiny].State)
            {
                case BoardState.Player1: Console.BackgroundColor = ConsoleColor.Red; break;
                case BoardState.Player2: Console.BackgroundColor = ConsoleColor.Blue; break;
                case BoardState.Draw: Console.BackgroundColor = ConsoleColor.DarkGray; break;
                default: Console.BackgroundColor = ConsoleColor.Black; break;
            }

            switch (fields[index].Value)
            {
                case 01: Console.Write(" O "); break;
                case 02: Console.Write(" X "); break;
                default: Console.Write("   "); break;
            }
            if (index % 9 == 8)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
                if (index % 27 == 26)
                {
                    Console.WriteLine(new string('-', 29));
                }
            }
            else if (index % 3 == 2)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write('|');
            }
        }
    }

    public int LastIndex { get; private set; }

    public int Round { get { return (Move + 1) >> 1; } }
    public int Move { get { return fields.Count(f => f.Value > 0) + 1; } }

    public bool OToMove { get { return (Move & 1) == 1; } }

    public BoardState State
    {
        get
        {
            foreach (var score in Score.All)
            {
                if (score.All(index => tinies[index].State == BoardState.Player1)) { return BoardState.Player1; }
                if (score.All(index => tinies[index].State == BoardState.Player2)) { return BoardState.Player2; }
            }
            return tinies.All(field => field.State != BoardState.None) ? BoardState.Draw : BoardState.None;
        }
    }

    public bool Apply(string move)
    {
        if (string.IsNullOrEmpty(move) || !move.StartsWith("place_move ")) { return false; }
        var parts = move.Split(' ');
        if (parts.Length != 3 || parts[1].Length != 1 || parts[2].Length != 1) { return false; }

        var col = parts[1][0] - '0';
        var row = parts[2][0] - '0';

        if (row < 0 || row > 8 || col < 0 || col > 8) { return false; }

        var active = ToTiny(row, col);

        if (MacroBoard[active] != -1) { return false; }

        var index = ToIndex(row, col); ;

        if (fields[index].Value > 0) { return false; }

        fields[index].Value = OToMove ? 1 : 2;

        LastIndex = index;

        return true;
    }
    public override string ToString()
    {
        return string.Join(",", fields.Select(f => f.Value));
    }


    public static int ToIndex(int row, int col)
    {
        return row * 9 + col;
    }

    public static int ToTiny(int row, int col)
    {
        return 3 * (row / 3) + (col / 3);
    }

    public static int ToTinyIndex(int index)
    {
        var c = index % 3;
        var r = (index / 9) % 3;
        return r * 3 + c;
    }

    public string GetGameUpdate()
    {
        var sb = new StringBuilder();

        sb.AppendFormat("update game round {0}", Round).AppendLine();
        sb.AppendFormat("update game move {0}", Move).AppendLine();
        sb.AppendFormat("update game field {0}", ToString()).AppendLine();
        sb.AppendFormat("update game macroboard {0}", ToMacro()).AppendLine();
        return sb.ToString();
    }
}
