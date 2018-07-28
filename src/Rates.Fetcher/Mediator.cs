using Rates.Core;
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
        private readonly IHandler<RateAdded> _rateAddedHandler;

        public Mediator(IHandler<RateAdded> rateAddedHandler)
        {
            _rateAddedHandler = rateAddedHandler;
        }

        public Task Send<T>(T @event)
        {
            switch (@event)
            {
                case RateAdded r:
                    return _rateAddedHandler.Handle(r);
                default:
                    throw new InvalidOperationException($"Cannot send event of type {@event.GetType().Name}");
            }
        }
    }
}
