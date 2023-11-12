namespace Common.Attributes
{
    /// <summary>
    /// Attribute used to associate a message type with a class implementing a message in the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class MessageAttribute : Attribute
    {
        /// <summary>
        /// Gets the message type associated with the attributed class.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageAttribute"/> class with the specified message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        public MessageAttribute(string messageType)
        {
            MessageType = messageType;
        }
    }
}
