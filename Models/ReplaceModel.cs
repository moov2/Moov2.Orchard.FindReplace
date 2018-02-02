using System.Collections.Generic;

namespace Moov2.Orchard.FindReplace.Models
{
    public class ReplaceModel
    {
        public string Find { get; set; }

        public string Replace { get; set; }

        public IList<int> ItemIds { get; set; }
    }
}