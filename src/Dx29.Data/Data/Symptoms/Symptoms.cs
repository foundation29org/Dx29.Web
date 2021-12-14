using System;
using System.Collections.Generic;

namespace Dx29.Data
{
    public partial class Symptom
    {
        public Symptom()
        {
            Sources = new List<SymptomSource>();
        }

        public string Id { get; set; }
        public string Status { get; set; }
        public DateTimeOffset LastUpdate { get; set; }

        public IList<SymptomSource> Sources { get; set; }
    }

    public class SymptomSource
    {
        public SymptomSource()
        {
        }
        public SymptomSource(string id, string name, string segments)
        {
            Id = id;
            Name = name;
            Segments = segments == null ? null : segments.Split(';');
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public IList<string> Segments { get; set; }
    }

    public class SymptomDesc : Symptom
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public IList<Reference> Categories { get; set; }
    }
}
