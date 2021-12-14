using System;
using System.Collections.Generic;

namespace Dx29.Web.Models
{
    public class NotesModel : List<NoteModel>
    {
    }

    public class NoteModel
    {
        public NoteModel() { }
        public NoteModel(string content)
        {
            Content = content;
            CreatedOn = DateTimeOffset.UtcNow;
            UpdatedOn = CreatedOn;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
    }
}
