using Rates.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core
{
    public interface IHandler<TEvent> where TEvent : Event
    {
        Task Handle(TEvent @event);
    }
}
