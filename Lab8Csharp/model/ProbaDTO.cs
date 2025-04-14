using System;

namespace Lab8Csharp.model
{
    [Serializable]
    public class ProbaDTO : Entity<long>
    {
        public string Distanta { get; set; }
        public string Stil { get; set; }
        public int? NrPar { get; set; }

        public ProbaDTO() {}

        public ProbaDTO(string distanta, string stil, int? nrPar)
        {
            Distanta = distanta;
            Stil = stil;
            NrPar = nrPar;
        }
    }
}