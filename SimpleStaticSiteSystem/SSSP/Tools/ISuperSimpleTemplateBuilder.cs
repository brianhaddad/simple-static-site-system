using SSClasses;

namespace SSSP.Tools
{
    public interface ISuperSimpleTemplateBuilder
    {
        void Build(StaticSiteProject project, string path, string env);
    }
}