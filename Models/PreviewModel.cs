using Orchard.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace Moov2.Orchard.FindReplace.Models
{
    public class PreviewModel
    {
        public string Find { get; set; }

        public IEnumerable<IContent> ContentItems { get; set; }

        public dynamic Display { get; set; }

        public bool HasMatches
        {
            get { return ContentItems != null && ContentItems.Any(); }
        }

        public int MatchCount
        {
            get { return ContentItems != null ? ContentItems.Count() : 0; }
        }
    }
}