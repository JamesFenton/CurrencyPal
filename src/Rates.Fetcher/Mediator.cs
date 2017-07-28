using Rates.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class Mediator
    {
        private readonly RateAddedHandler _rateAddedHandler;

        public Mediator(RateAddedHandler rateAddedHandler)
        {
            _rateAddedHandler = rateAddedHandler;
        }

        public void Send(Event e)
        {
            switch (e)
            {
                case RateAdded r:
                    _rateAddedHandler.Handle(r);
                    break;
                default:
                    throw new InvalidOperationException($"Cannot send event of type {e.GetType().Name}");
            }
        }
    }
}
