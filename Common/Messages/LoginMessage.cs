using Common.Attributes;

namespace Common.Messages
{
    [Message("Login")]
    public class LoginMessage : Message
    {
        public LoginMessage() : base("Login") { }

        override public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
