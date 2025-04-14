using System;

namespace Lab8Csharp.model
{
    [Serializable]
    public class Entity<ID>
    {
        public ID Id { get; set; }

        public Entity() {}

        public Entity(ID id)
        {
            Id = id;
        }
    }
}