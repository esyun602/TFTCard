using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace MessageSystem
{
    public class MessageSystemManager : MonoBehaviour
    {
        private Dictionary<object, MethodInfo> methodInfoDict = new();
        public delegate void subscribeCallback<T>(T m) where T : Message;
        internal static void ExecuteSubscribeCallback(object callback, object param)
        {
            try
            {
                if (Instance.methodInfoDict.TryGetValue(callback, out var info))
                {
                    info.Invoke(callback, new object[] { param });
                }
                else
                {
                    var method = callback.GetType().GetMethod("Invoke");
                    method.Invoke(callback, new object[] { param });
                    Instance.methodInfoDict[callback] = method;
                }
            }
            catch (Exception e)
            {
                    Debug.LogError(e);
            }
        }

        internal static MessageSystemManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region publish
        internal List<Notice> NoticePublishRequestList = new();
        #endregion

        #region send 
        internal List<KeyValuePair<Notice, IMessageReceiver>> NoticeSendRequestList = new();
        #endregion

        #region subscribe
        internal Dictionary<Type, List<object>> NoticeSubscriber = new();

        internal List<KeyValuePair<Type, object>> NoticeSubscribeRequest = new();

        internal List<KeyValuePair<Type, object>> NoticeUnsubscribeRequest = new();
        #endregion
        private MessageSystemManager()
        {

        }


        private void Update()
        {
            //Preprocess();
            ProcessSubscribeRequest();

            ProcessNotice();
        }

        /*
        private void Preprocess()
        {
            OnProcessingNoticePublishRequestList = new(NoticePublishRequestList);
            NoticePublishRequestList.Clear();
            OnProcessingNoticeSendRequestList = new(NoticeSendRequestList);
            NoticeSendRequestList.Clear();
        }
        */

        private void ProcessSubscribeRequest()
        {
            foreach (var kvp in NoticeSubscribeRequest)
            {
                var noticeType = kvp.Key;
                var callback = kvp.Value;
                if (NoticeSubscriber.TryGetValue(noticeType, out var subscribingCallbackList))
                {
                    subscribingCallbackList.Add(callback);
                }
                else
                {
                    NoticeSubscriber.Add(noticeType, new());
                    NoticeSubscriber[noticeType].Add(callback);
                }
            }
            NoticeSubscribeRequest.Clear();

            foreach (var kvp in NoticeUnsubscribeRequest)
            {
                var noticeType = kvp.Key;
                var callback = kvp.Value;
                if (NoticeSubscriber.TryGetValue(noticeType, out var subscribingCallbackList))
                {
                    subscribingCallbackList.Remove(callback);
                    methodInfoDict.Remove(callback);
                }
            }
            NoticeUnsubscribeRequest.Clear();
        }
        private void ProcessNotice()
        {
            //fix - 메세지 체인 타이밍이 최대 1frame으로 보장되도록 수정
            for (var i = 0; i < NoticeSendRequestList.Count; i++)
            {
                var sendRequest = NoticeSendRequestList[i];
                var message = sendRequest.Key;
                var target = sendRequest.Value;
                target?.CatchMessage(message);
            }
            NoticeSendRequestList.Clear();

            
            //fix - 메세지 체인 타이밍이 최대 1frame으로 보장되도록 수정
            for (var i = 0; i < NoticePublishRequestList.Count; i++)
            {
                var message = NoticePublishRequestList[i];
                if (NoticeSubscriber.TryGetValue(message.GetType(), out var callbackList))
                {
                    foreach (var callback in callbackList)
                    {
                        ExecuteSubscribeCallback(callback, message);
                    }
                }
            }
            NoticePublishRequestList.Clear();
        }
    }
}
