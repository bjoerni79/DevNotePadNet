namespace DevNotePad.ViewModel
{
    /// <summary>
    /// JSON Action commands 
    /// </summary>
    internal enum JsonOperation
    {
        Format,
        ToTree,
        ToText
    }

    /// <summary>
    /// XML Action commands
    /// </summary>
    internal enum XmlOperation
    {
        Format,
        ToTree,
        ToText
    }

    /// <summary>
    /// File Action commands
    /// </summary>
    internal enum FileOperation
    {
        New,
        Open,
        OpenBinary,
        Save,
        SaveAs,
        SaveBinary,
        SaveAsBinary,
        Reload
    }

    internal enum XmlToolFeature
    {
        SchemaValidation,
        XPathQuery,
        XSltTransformation
    }
}


