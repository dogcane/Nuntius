using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nuntius.Core.Messages
{
    public record RenderedMessage(string Subject, string Content)
    {
        public RenderedMessage() : this(string.Empty, string.Empty) { }
    }
}
