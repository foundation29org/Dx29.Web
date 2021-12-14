﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dx29.Data
{
    public class Resource
    {
        public Resource()
        {
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public Resource(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public string Status { get; set; } // undefined, selected, unselected
        public IDictionary<string, string> Properties { get; set; } // path, score, error, errorMessage

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }

    public class ResourceComparer<TResource> : IEqualityComparer<TResource> where TResource : Resource
    {
        public bool Equals(TResource x, TResource y)
        {
            return x.Id.EqualsNoCase(y.Id);
        }

        public int GetHashCode([DisallowNull] TResource resource)
        {
            return resource.Id.GetHashCode();
        }
    }
}
