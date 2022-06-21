using DevNotePad.Features.Shared;

namespace DevNotePad.Features.Xml
{
    public interface IXmlComponent
    {
        Task<string> FormatterAsync(string xmlText);

        Task<IEnumerable<ItemNode>> ParseToTreeAsync(string xmlText);
    }
}
