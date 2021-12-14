using System;
using System.Collections.Generic;

using Dx29.Data;

namespace Dx29.Web.UI.Components
{
    partial class MoreInfo
    {
        private List<XRefs> GetXRefs(DiffDisease Item)
        {
            List<XRefs> xrefs = new List<XRefs>();
            foreach (var xref in Item.XRefs)
            {
                if (xref.IndexOf("ORPHANET") > -1)
                {
                    xrefs.Add(new XRefs()
                    {
                        Reference = xref.Replace("ORPHANET","Orphanet"),
                        Info = new XRefData()
                        {
                            Id = xref.Split(":")[1],
                            Link = "https://www.orpha.net/consor/cgi-bin/OC_Exp.php?Expert=" + xref.Split(":")[1] + "&lng=en"
                        }
                    });
                }
                else if (xref.IndexOf("OMIM") > -1)
                {
                    xrefs.Add(new XRefs()
                    {
                        Reference = xref,
                        Info = new XRefData()
                        {
                            Id = xref.Split(":")[1],
                            Link = "https://omim.org/entry/" + xref.Split(":")[1]
                        }
                    });

                }
            }
            return xrefs;
        }
    }
}
