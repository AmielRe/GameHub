using Common.Attributes;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : IMessage
    {
        public UpdateResourcesMessage() { }

        public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
