using System;

namespace Lab8Csharp.model
{
    [Serializable]
    public class Participant : Entity<long>
    {
        public string Name { get; set; }
        public int? Age { get; set; }

        public Participant() {}

        public Participant(string name, int? age)
        {
            Name = name;
            Age = age;
        }
    }
}