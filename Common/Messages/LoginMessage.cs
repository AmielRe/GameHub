using Common.Attributes;

namespace Common.Messages
{
    [Message("Login")]
    public class LoginMessage : IMessage
    {
        public LoginMessage() { }

        public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
