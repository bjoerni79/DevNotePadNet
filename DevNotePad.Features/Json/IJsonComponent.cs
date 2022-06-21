using DevNotePad.Features.Shared;

namespace DevNotePad.Features.Json
{
    public interface IJsonComponent
    {
        string Formatter(string jsonText);

        //string ParseToString(string jsonText);

        ItemNode ParseToTree(string jsonText);
    }
}
