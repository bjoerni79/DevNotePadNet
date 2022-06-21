namespace DevNotePad.Features.Text
{
    public interface ITextFormatComponent
    {
        string CountLength(string text);

        string CountLength(string text, bool inHexRepresentation);

        string GroupString(string text);

        string ToUpper(string text);

        string Trim(string text);

        string ToLower(string text);

        string SplitString(string text);

        string SplitString(string text, TextFormatComponentSettings settings);

        string FormatHex(string text);

    }
}
