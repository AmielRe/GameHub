using Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    [Message("SendGift")]
    internal class SendGiftMessage : IMessage
    {
        public SendGiftMessage() { }
        public void Handle(dynamic message)
        {
            throw new NotImplementedException();
        }
    }
}
