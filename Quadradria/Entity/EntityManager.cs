using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quadradria.Entity
{
    public enum EntityType : ushort { Base = 0, Item = 1, Human = 2, Horse = 5 }

    class EntityManager
    {
        public static BaseEntity[] idArray = new BaseEntity[Enum.GetValues(typeof(EntityType)).Cast<ushort>().Max()+1];

        public static void AddEntityType(BaseEntity entity)
        {
            Console.WriteLine("Register Entity: " + entity.EntType);
            idArray[(ushort)entity.EntType] = entity;
        }

        public static void Init()
        {
            AddEntityType(new Human());
        }

        public static BaseEntity Spawn(EntityType entid)
        {
            BaseEntity entity = idArray[(int)entid];
            return entity.Create();
        }
    }
}
