using Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    [Message("UpdateResources")]
    internal class UpdateResourcesMessage : IMessage
    {
        public UpdateResourcesMessage() { }

        public void Handle(dynamic message)
        {
            throw new NotImplementedException();
        }
    }
}
