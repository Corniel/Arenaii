using System;

namespace Arenaii.RiddlesIo.Golad.Model
{
    public struct Move: IEquatable<Move>
    {
        private const int Mask = short.MaxValue;

        public static readonly Move Pass;

        public static Move Kill(Cell cell) => Kill(cell.Index);
        public static Move Kill(int index) => new Move((index << 1) | 1);
       
        public static Move Birth(int child, long father, long mother)
        {
            var swap = father > mother;
            var min = swap ? mother: father;
            var max = swap ? father : mother;
            
            long val = child << 1;
            val |= min << 22;
            val |= max << 43;
            return new Move(val);
        }


        private Move(long val)
        {
            value = val;
        }

        private readonly long value;

        public int Index => (int)(value >> 1) & Mask;
        public int Father => (int)(value >> 22) & Mask;
        public int Mother => (int)(value >> 43) & Mask;

        public bool IsPass() => value == 0;
        public bool IsKill() => (value & 1) == 1;
        public bool IsBirth() => !IsPass() && !IsKill();

        public override bool Equals(object obj) => obj is Move move && Equals(move);
        public bool Equals(Move other) => value == other.value;
        public override int GetHashCode() => value.GetHashCode();

        public override string ToString()
        {
            if(IsPass())
            {
                return "Pass";
            }
            if(IsKill())
            {
                return $"Kill {Index}";
            }
            else
            {
                return $"Birth {Index}, Parents: {Father}, {Mother}";
            }
        }
    }
}
