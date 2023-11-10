namespace Common.Messages
{
    public interface IMessage
    {
        void Handle(dynamic message);
    }
}
