using Common.Attributes;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : IMessage
    {
        public SendGiftMessage() { }
        public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
