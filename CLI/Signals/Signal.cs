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

        public int Dispatch()
        {
            try
            {
                Console.WriteLine("Starting application cleanup...");
                subscribers.Reverse();
                subscribers.ForEach(s => s.Invoke());
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}
