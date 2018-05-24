using Quadradria.Entity;
using Quadradria.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Enviroment
{
    //Stores only entities of a collection of smaller chunks
    class Megachunk
    {
        public const int SIZE = 32;

        public List<RawEntity>[,] chunks;

        public Megachunk(byte[] data)
        {
            chunks = new List<RawEntity>[SIZE, SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    chunks[i, j] = new List<RawEntity>();
                }
            }
        }

        public void Import(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream)) {
                uint number = reader.ReadUInt32();
                for (int i = 0; i < number; i++)
                {
                    uint length = reader.ReadUInt32();
                    EntityType type = (EntityType)reader.ReadUInt16();
                    float x = reader.ReadSingle();
                    float y = reader.ReadSingle();
                    byte[] entData = reader.ReadBytes((int)length);

                    RawEntity rohling = new RawEntity(type, x, y, entData);
                    List<RawEntity> chunk = chunks[(int)(x/32), (int)(y/32)];
                    chunk.Add(rohling);
                }
            }
        }

        public byte[] Export(World world, int x, int y)
        {
            MemoryStream stream = new MemoryStream();



            return new byte[0];
        }

        public class RawEntity
        {
            public EntityType type;
            public float x, y;
            public byte[] data;

            public RawEntity(EntityType type, float x, float y, byte[] data)
            {
                this.type = type;
                this.x = x;
                this.y = y;
                this.data = data;
            }
        }
    }
}
