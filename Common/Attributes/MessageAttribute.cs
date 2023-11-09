namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MessageAttribute : Attribute
    {
        public string MessageType { get; }

        public MessageAttribute(string messageType)
        {
            MessageType = messageType;
        }
    }
}
