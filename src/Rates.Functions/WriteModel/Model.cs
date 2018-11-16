using Microsoft.WindowsAzure.Storage.Table;
using Rates.Functions.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.WriteModel
{
    public abstract class Model : TableEntity
    {
        private readonly List<Event> _events = new List<Event>();

        protected void AddEvent(Event e) => _events.Add(e);

        public List<Event> GetEvents() => new List<Event>(_events);

        public void ClearEvents() => _events.Clear();
    }
}
