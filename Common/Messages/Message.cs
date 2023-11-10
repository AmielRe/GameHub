namespace Common.Messages
{
    public abstract class Message : IMessage
    {
        public string MsgType {  get; }

        public Message(string msgType)
        {
            MsgType = msgType;
        }

        public abstract void Handle(dynamic message);
    }
}
