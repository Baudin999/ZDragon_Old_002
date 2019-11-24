using System;
using System.Collections.Generic;

namespace CLI
{
    public class Signal
    {
        private List<Action> subscribers = new List<Action>();
        public void Subscribe(Action subscriber)
        {
            this.subscribers.Add(subscriber);
        }

        public void Dispatch()
        {
            subscribers.ForEach(s => s.Invoke());
        }
    }
}
