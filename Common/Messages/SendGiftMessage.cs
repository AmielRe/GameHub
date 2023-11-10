using Common.Attributes;

namespace Common.Messages
{
    [Message("SendGift")]
    public class SendGiftMessage : Message
    {
        public SendGiftMessage() : base("SendGift") { }
        override public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
