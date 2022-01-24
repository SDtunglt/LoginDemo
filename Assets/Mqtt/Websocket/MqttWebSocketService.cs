using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using UnityEngine;

public class MqttWebSocketService
{
    public static List<SubscribedTopic> SubscribedTopics = new List<SubscribedTopic>();

    public delegate void OnConnectedCallback();

    public delegate void OnDisconnectedCallback();

    public delegate void OnPublishMsgReceivedCallBack(System.IntPtr topicPtr, System.IntPtr msgPtr);

#if UNITY_WEBGL
    [DllImport("__Internal")]
    public static extern void MqttConnect(string clientId, string host, string userName, string passWord);

    [DllImport("__Internal")]
    public static extern void MqttSetConnected(OnConnectedCallback callback);

    [DllImport("__Internal")]
    public static extern void MqttSetDisconnected(OnDisconnectedCallback callback);

    [DllImport("__Internal")]
    public static extern void MqttSetMsgPublishReceived(OnPublishMsgReceivedCallBack callback);

    [DllImport("__Internal")]
    public static extern void Subscribe(string topic, int quos);

    [DllImport("__Internal")]
    public static extern void UnSubscribe(string topics);

    [DllImport("__Internal")]
    public static extern void Publish(string topic, string message, int qos);

    [DllImport("__Internal")]
    public static extern void CloseClient();
#endif

    private static bool isInitialized;

    public static bool isConnected;

    public static Action<string, byte[]> OnPublishMsgReceived;
    public static Action OnConnected;
    public static Action OnDisconnected;

    public static void ConnectMqtt(string clientId, string host, string userName, string passWord)
    {
#if UNITY_WEBGL
        if (!isInitialized)
        {
            MqttSetConnected(DelegateOnConnected);
            MqttSetDisconnected(DelegateOnDisconnected);
            MqttSetMsgPublishReceived(DelegatePublishMsgReceived);
            isInitialized = true;
        }

        MqttConnect(clientId, host, userName, passWord);
#endif
    }

    [MonoPInvokeCallback(typeof(OnConnectedCallback))]
    public static void DelegateOnConnected()
    {
        isConnected = true;
        SubscribeTopics(SubscribedTopics.Select(s => s.topic).ToArray(), SubscribedTopics.Select(s => s.qos).ToArray());
        OnConnected?.Invoke();
    }

    [MonoPInvokeCallback(typeof(OnDisconnectedCallback))]
    public static void DelegateOnDisconnected()
    {
#if UNITY_WEBGL
        isConnected = false;
        CloseClient();
        OnDisconnected?.Invoke();
#endif
    }

    [MonoPInvokeCallback(typeof(OnPublishMsgReceivedCallBack))]
    public static void DelegatePublishMsgReceived(System.IntPtr topicPtr, System.IntPtr msgPtr)
    {
        try
        {
            var topic = Marshal.PtrToStringAuto(topicPtr);
            var msg = Marshal.PtrToStringAuto(msgPtr);
            var bytes = msg.Split(',').Select(byte.Parse).ToArray();
            OnPublishMsgReceived?.Invoke(topic, bytes);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    public static void SubscribeTopics(string[] topicNames, byte[] Qoss)
    {
        if (isConnected)
        {
#if UNITY_WEBGL
            for (int i = 0; i < topicNames.Length; i++)
            {
                if (!SubscribedTopics.Exists(s => s.topic == topicNames[i]))
                {
                    SubscribedTopics.Add(new SubscribedTopic
                    {
                        topic = topicNames[i],
                        qos = Qoss[i]
                    });
                }
                Subscribe(topicNames[i], Qoss[i]);
            }

#endif
        }
    }

    public static void UnSubscribe(string[] topicNames)
    {
        if (isConnected)
        {
#if UNITY_WEBGL
            var topics = string.Join("|", topicNames);
            UnSubscribe(topics);
            foreach (var name in topicNames)
            {
                var index = SubscribedTopics.FindIndex(s => s.topic == name);
                if (index != -1)
                {
                    SubscribedTopics.RemoveAt(index);
                }
            }
#endif
        }
    }

    public static void Publish(string topic, byte[] bytes, byte qos)
    {
        if (isConnected)
        {
#if UNITY_WEBGL
            var msg = string.Join("|", bytes);
            Publish(topic, msg, qos);
#endif
        }
    }

    public static void DisconnectMqtt()
    {
#if UNITY_WEBGL
        if (isConnected)
        {
            isConnected = false;
            CloseClient();
        }
#endif
       
    }
}

[Serializable]
public struct SubscribedTopic
{
    public string topic;
    public byte qos;
}