using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Project
{
    public class ModuleStream : IDisposable
    {

        private readonly Subject<ModuleStreamMessage> moduleMessageSubject;
        private readonly IDictionary<string, IDisposable> subscribers;
        private List<Action<ModuleStreamMessage>> once;

        public ModuleStream()
        {
            moduleMessageSubject = new Subject<ModuleStreamMessage>();
            subscribers = new Dictionary<string, IDisposable>();
            once = new List<Action<ModuleStreamMessage>>();
        }

        public void Dispose()
        {
            if (moduleMessageSubject != null)
            {
                moduleMessageSubject.Dispose();
            }

            foreach (var subscriber in subscribers)
            {
                subscriber.Value.Dispose();
            }

            once = new List<Action<ModuleStreamMessage>>();
        }


        /// <summary>
        /// Publish a ModuleStreamMessage, triggering the subscriptions.
        /// </summary>
        /// <param name="moduleMessage"></param>
        public void Publish(ModuleStreamMessage moduleMessage)
        {
            moduleMessageSubject.OnNext(moduleMessage);

            
            foreach (var action in once)
            {
                action(moduleMessage);
            }
            once = new List<Action<ModuleStreamMessage>>();
        }


        /// <summary>
        /// Subscribe to all publications of a ModuleStreamMessage
        /// </summary>
        /// <param name="subscriberName"></param>
        /// <param name="action"></param>
        public Action Subscribe(string subscriberName, Action<ModuleStreamMessage> action)
        {
            if (!subscribers.ContainsKey(subscriberName))
            {
                subscribers.Add(subscriberName, moduleMessageSubject.Subscribe(action));

                return () =>
                {
                    subscribers.Remove(subscriberName);
                };
            }
            return () => { };
        }


        /// <summary>
        /// Only subscribe to messages with a certain message type. This way you
        /// can filter out those messages which will be handled by other systems.
        /// </summary>
        /// <param name="subscriberName">Simple unique name of the subscriber</param>
        /// <param name="messageType">The message type you would like to subscribe to</param>
        /// <param name="action">The action which you'd want to take one a valid message is published.</param>
        public Action Subscribe(string subscriberName, MessageType messageType, Action<ModuleStreamMessage> action)
        {
            if (!subscribers.ContainsKey(subscriberName))
            {
                subscribers.Add(
                    subscriberName,
                    moduleMessageSubject
                        .Where(msm => msm.MessageType == messageType)
                        .Subscribe(action)
                );

                return () =>
                {
                    subscribers.Remove(subscriberName);
                };
            }
            return () => { };
        }

        public void Once(Action<ModuleStreamMessage> action)
        {
            once.Add(action);
        }



    }



}
