namespace NitroxModel.Server
{
    public enum ServerSerializerMode
    {
        PROTOBUF,
        JSON
#if DEBUG
        ,JSON_DEBUG
#endif
    }
}
