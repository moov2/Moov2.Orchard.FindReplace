using Orchard.UI.Resources;

namespace Moov2.Orchard.FindReplace
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            manifest.DefineScript("FindReplacePreview").SetUrl("preview.js", "preview.js").SetDependencies(new string[] { "jQuery" });
        }
    }
}