using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Messages
{
    public interface IMessage
    {
        void Handle(dynamic message);
    }
}
