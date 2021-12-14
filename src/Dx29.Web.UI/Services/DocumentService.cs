using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Dx29.Data;

namespace Dx29.Web.UI.Services
{
    public class DocumentService
    {
        public DocumentService(Dx29Client dx29Client)
        {
            Dx29Client = dx29Client;
        }

        public Dx29Client Dx29Client { get; }

        public async Task<DocEntities> GetDocumentEntitiesAsync(DocAnnotations doc)
        {
            var docTerms = new DocEntities();
            foreach (var seg in doc.Segments)
            {
                foreach (var ann in seg.Annotations)
                {
                    if (ann.Links != null)
                    {
                        var terms = await ProcessLinksAsync(ann.Links);
                        string key = $"{seg.Id}A{ann.Id}";
                        docTerms.Entities.Add(key, terms);
                        docTerms.Scores.Add(key, ann.ConfidenceScore);
                    }
                }
            }
            return docTerms;
        }

        public async Task<IList<DocEntity>> GetDocEntitiesAsync(Annotation annotation)
        {
            if (annotation.Links != null)
            {
                return await ProcessLinksAsync(annotation.Links);
            }
            return Array.Empty<DocEntity>();
        }

        private async Task<IList<DocEntity>> ProcessLinksAsync(IList<AnnotationLink> links)
        {
            var terms = new List<DocEntity>();

            var hpo = links.Where(r => r.DataSource == "HPO").FirstOrDefault();
            if (hpo != null)
            {
                string id = hpo.Id;
                terms.AddRange(GetTermModels(await Dx29Client.GetTermsAsync(id)));
            }
            var omim = links.Where(r => r.DataSource == "OMIM").FirstOrDefault();
            if (omim != null)
            {
                string id = $"{omim.DataSource}:{omim.Id}";
                terms.AddRange(GetTermModels(await Dx29Client.GetTermsAsync(id)));
                terms.Add(new DocEntity { Id = id });
            }
            foreach (var link in links.Take(10))
            {
                string id = $"{link.DataSource}:{link.Id}";
                if (link.DataSource == "HPO")
                {
                    id = link.Id;
                }
                terms.Add(new DocEntity { Id = id });
            }

            return terms.Distinct().ToList();
        }

        static private IEnumerable<DocEntity> GetTermModels(IDictionary<string, IList<Term>> terms)
        {
            foreach (var item in terms.First().Value)
            {
                yield return new DocEntity
                {
                    Id = item.Id,
                    Name = item.Name,
                    Desc = item.Desc
                };
            }
        }
    }
}
