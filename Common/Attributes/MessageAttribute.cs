using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class MessageAttribute : Attribute
    {
        public string MessageType { get; }

        public MessageAttribute(string messageType)
        {
            MessageType = messageType;
        }
    }
}
