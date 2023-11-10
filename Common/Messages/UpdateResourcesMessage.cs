using Common.Attributes;

namespace Common.Messages
{
    [Message("UpdateResources")]
    public class UpdateResourcesMessage : Message
    {
        public UpdateResourcesMessage() : base("UpdateResources") { }

        override public void Handle(dynamic message)
        {
            Console.WriteLine(message.type);
        }
    }
}
