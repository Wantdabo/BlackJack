using Network;
using Network.External;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Link : MonoBehaviour
{
    public static Link link;
    private ExternalCall externalCall;
    private SocketInfo socketInfo;
    private Queue<SocketInfo> receiveQueue;
    private string ip;
    private int port;

    public static Link Instance
    {
        get
        {
            if (link == null)
                link = Camera.main.GetComponent<Link>();

            return link;
        }
    }

    private Link()
    {
        externalCall = new ExternalCall();
        receiveQueue = new Queue<SocketInfo>();
    }

    private void Start()
    {
        StartCoroutine("HandleReceive");
    }

    public void Connect(string _ip, int _port)
    {
        ip = _ip;
        port = _port;
        socketInfo = externalCall.Connect(AsyncReceive, port, ip);
    }

    public void ReConnect()
    {
        socketInfo.socket.Close();
        socketInfo = externalCall.Connect(AsyncReceive, port, ip);
    }

    public void AsyncReceive(SocketInfo _socketInfo)
    {
        receiveQueue.Enqueue(_socketInfo);
    }

    public void Send(BufferRW _bufferRW) {
        byte[] buffer = _bufferRW.EndWrite();

        externalCall.Call(buffer, socketInfo);
    }

    private IEnumerator HandleReceive()
    {
        while (true)
        {
            if (receiveQueue.Count > 0) {
                SocketInfo socketInfo = receiveQueue.Dequeue();
                BufferRW brw = new BufferRW();
                brw.BeginRead(socketInfo.buffer);
                int proto = brw.ReadInt16();
                int session = brw.ReadInt32();
                int money = brw.ReadInt32();
                Lua.Instance.HandleSocketInfo(socketInfo);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnApplicationQuit() {
        socketInfo.socket.Close();
    }
}
