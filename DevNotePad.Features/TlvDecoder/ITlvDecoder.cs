namespace DevNotePad.Features.TlvDecoder
{
    public interface ITlvDecoder
    {
        Tlv Decode(IEnumerable<byte> tlvCoding);
    }
}
