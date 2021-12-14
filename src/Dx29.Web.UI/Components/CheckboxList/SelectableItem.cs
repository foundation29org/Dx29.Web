using System;
using System.Linq;
using System.Collections.Generic;

namespace Dx29.Web.UI.Components
{
    public interface ISelectableItem
    {
        string Name { get; set; }
        bool Selected { get; set; }
    }

    public class SelectableItem : ISelectableItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }

        static public IList<SelectableItem> CreateList(params string[] names)
        {
            return names.Select(r => new SelectableItem { Id = r, Name = r, Selected = false }).ToList();
        }
        static public IList<SelectableItem> CreateList(bool selected, params string[] names)
        {
            return names.Select(r => new SelectableItem { Id = r, Name = r, Selected = selected }).ToList();
        }
        static public IList<SelectableItem> CreateList(IDictionary<string, string> values)
        {
            return values.Select(r => new SelectableItem { Id = r.Key, Name = r.Value, Selected = false }).ToList();
        }
    }
}
