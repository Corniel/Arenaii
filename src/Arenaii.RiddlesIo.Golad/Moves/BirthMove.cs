﻿using Arenaii.RiddlesIo.Golad.Model;

namespace Arenaii.RiddlesIo.Golad.Moves
{
    public struct BirthMove : IMove
    {
        public BirthMove(Cell child, Cell father, Cell mother)
        {
            Child = child;
            Father = father;
            Mother = mother;
        }
        public Cell Child { get; }
        public Cell Father { get; }
        public Cell Mother { get; }

        public bool Apply(Cells cells)
        {
            // We can't revoke a cell that is alive.
            if (Child.IsAlive)
            {
                return false;
            }
            // It should be two different cells that die.
            if (Father == Mother)
            {
                return false;
            }
            var player = cells.P0ToMove ? Player.Player0 : Player.Player1;

            // Wrong owner (of not even alive).
            if (Mother.Owner != player || Father.Owner != player)
            {
                return false;
            }
            cells.State[Father.Index] = Player.None;
            cells.State[Mother.Index] = Player.None;
            cells.State[Child.Index] = player;
            return true;
        }

        public override string ToString() => $"Birth {Child.Row},{Child.Col} {Father.Row},{Father.Col} {Mother.Row},{Mother.Col}";
    }
}
