using Microsoft.WindowsAzure.Storage.Table;
using Rates.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core.WriteModel
{
    public abstract class Model : TableEntity
    {
        private readonly List<Event> _events = new List<Event>();

        protected void AddEvent(Event e) => _events.Add(e);

        public List<Event> GetEvents() => new List<Event>(_events);

        public void ClearEvents() => _events.Clear();
    }
}
