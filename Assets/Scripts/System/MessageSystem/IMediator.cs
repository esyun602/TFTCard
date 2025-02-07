using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageSystem
{
    public interface IMediator
    {
        public void Subscribe<T>(MessageSystemManager.subscribeCallback<T> callback) where T : Message;
        public void Unsubscribe<T>(MessageSystemManager.subscribeCallback<T> callback) where T : Message;
        public void Publish(Message message);
        public void PublishSync(Message message);
        public void Send(Message message, IMessageReceiver target);
        public void SendSync(Message message, IMessageReceiver target);
    }


}