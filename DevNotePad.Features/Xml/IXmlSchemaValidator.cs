namespace DevNotePad.Features.Xml
{
    public interface IXmlSchemaValidator
    {
        Task<SchemaCompareResult> CompareAsync(SchemaCompareRequest request);
    }
}
