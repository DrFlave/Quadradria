using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment.Generators
{
    interface IGenerator
    {
        int Seed { get; set; }
        void GenerateChunk(Chunk chunk);
    }
}
