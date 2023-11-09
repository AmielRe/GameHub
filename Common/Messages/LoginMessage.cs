using Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    [Message("Login")]
    internal class LoginMessage : IMessage
    {
        public LoginMessage() { }

        public void Handle(dynamic message)
        {
            throw new NotImplementedException();
        }
    }
}
