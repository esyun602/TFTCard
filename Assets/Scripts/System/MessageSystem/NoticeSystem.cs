using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageSystem
{
    public class NoticeSystem : IMediator
    {
        public static IMediator Instance { get; } = new NoticeSystem();

        private NoticeSystem()
        {

        }
        public void Subscribe<T>(MessageSystemManager.subscribeCallback<T> callback) where T : Message
        {
            if (!typeof(T).IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            MessageSystemManager.Instance.NoticeSubscribeRequest.Add(
                new (typeof(T), callback));
        }
        public void Unsubscribe<T>(MessageSystemManager.subscribeCallback<T> callback) where T : Message
        {
            if (!typeof(T).IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            MessageSystemManager.Instance.NoticeUnsubscribeRequest.Add(
                new (typeof(T), callback));
        }

        public void Publish(Message notice)
        {
            Debug.Log($"Published Notice: {notice}");
            if (!notice.GetType().IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            MessageSystemManager.Instance.NoticePublishRequestList.Add(notice as Notice);
        }
        public void PublishSync(Message notice)
        {
            Debug.Log($"Published Sync Notice: {notice}");
            if (!notice.GetType().IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            if (MessageSystemManager.Instance.NoticeSubscriber.TryGetValue(notice.GetType(), out var callbackList))
            {
                foreach (var callback in callbackList)
                {
                    MessageSystemManager.ExecuteSubscribeCallback(callback, notice);
                }
            }
        }
        public void Send(Message notice, IMessageReceiver target)
        {
            if (!notice.GetType().IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            MessageSystemManager.Instance.NoticeSendRequestList.Add(new KeyValuePair<Notice, IMessageReceiver>(notice as Notice, target));
        }
        public void SendSync(Message notice, IMessageReceiver target)
        {
            if (!notice.GetType().IsSubclassOf(typeof(Notice)))
            {
                return;
            }
            target.CatchMessage(notice);
        }

    }

}
