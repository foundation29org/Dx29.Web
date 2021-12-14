using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public class DocEntities
    {
        public DocEntities()
        {
            Entities = new Dictionary<string, IList<DocEntity>>();
            Scores = new Dictionary<string, double>();
        }

        public Dictionary<string, IList<DocEntity>> Entities { get; set; }
        public Dictionary<string, double> Scores { get; set; }

        public string GetScore(string key)
        {
            return Scores[key].ToString("0.00");
        }
    }

    public class DocEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }

        public bool IsEmpty => $"{Name}{Desc}".Length == 0;
    }
}
