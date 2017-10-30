using Arenaii.RiddlesIo.Golad.Data;
using System;

namespace Arenaii.RiddlesIo.Golad
{
    public class Program : Simulator<GoladCompetition, GoladSettings>
    {
        public Program()
        {
            Engine = new GoladEngine();
        }

        static void Main(string[] args)
        {
            var program = new Program();
            program.Run(args);
        }
    }
}
