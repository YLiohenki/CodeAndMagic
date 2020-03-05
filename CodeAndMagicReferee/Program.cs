using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAndMagicReferee
{
    class Program
    {
        static Referee referee = new Referee();
        static void Main(string[] args)
        {
            var result = referee.CompareWeights(new List<double>(), @"..\..\..\CodeAndMagic\bin\Debug\codeAndMagic.exe", new List<double>(), @"..\..\..\CodeAndMagic\bin\Debug\codeAndMagic.exe");
            Console.WriteLine("RESULT {0}", result);
            Console.ReadKey();
        }
    }
}
