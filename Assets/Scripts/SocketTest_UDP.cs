using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;


public class SocketTest_UDP : MonoBehaviour
{
    // UDP Variables
    Thread receiveThread;
    UdpClient client;
    public readonly int port = 27001;

    private string dataReceived = "EMPTY";

    // Modify TextMeshPro
    public TMP_Text worldText;

    // Start is called before the first frame update
    void Start()
    {
        InitUDPSocket();
    }

    // Update is called once per frame
    void Update()
    {
        if (worldText.text != dataReceived)
        {
            worldText.text = dataReceived;
            print("Current label: " + dataReceived);
        }

        if (worldText.text == "CONFIRM")
        {
            worldText.color = new Color32(0, 255, 0, 255);
        }
        else
            worldText.color = new Color32(255, 255, 255, 255);
    }

    private void InitUDPSocket()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        /*
         Background threads are identical to foreground threads, except that background threads do not prevent a process from terminating.
         Once all foreground threads belonging to a process have terminated, the common language runtime ends the process. 
         Any remaining background threads are stopped and do not complete.
         */
        receiveThread.Start();

    }

    private void GetInfo()
    {

    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        // Change while true to while client != null
        // Turned off client first before terminating thread in OnDestroy
        while (client != null)
        {
            try
            {
                // IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //0.0.0.0 is the equivalent of ANY IP
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
                byte[] buffer = client.Receive(ref anyIP);          // Getting UDP Data in Bytes from Python

                // When connection is closed, buffer will receive ""
                if (buffer == null)
                    return;

                dataReceived = Encoding.UTF8.GetString(buffer);     // Convert Bytes into String
                print("Data Received: " + dataReceived);
            }
            catch (Exception err)
            {
                print(err);
                if (client != null)
                    print("UDP Socket Exception Error: " + err);
                else
                    print("Thread is about to terminate . . .");
            }
        }
    }

    //private void OnApplicationQuit()
    //{
    //    if (receiveThread.IsAlive || receiveThread != null)
    //    {
    //        if (receiveThread.Join(100))
    //        {
    //            print("UDP Thread has closed successfully - Quit");
    //        }
    //        else
    //        {
    //            print("UDP Thread did not close in 100ms, abort - Quit");
    //            receiveThread.Abort();
    //        }
    //    }
    //    client.Close();

    //}

    //private void OnDisable()
    //{
    //    if (receiveThread.IsAlive || receiveThread != null)
    //    {
    //        if (receiveThread.Join(100))
    //        {
    //            print("UDP Thread has closed successfully - Disable");
    //        }
    //        else
    //        {
    //            print("UDP Thread did not close in 100ms, abort - Disable");
    //            receiveThread.Abort();
    //        }

    //    }
    //    client.Close();
    //}

    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();

            client = null;
        }
        if (receiveThread.IsAlive || receiveThread != null)
        {
            if (receiveThread.Join(100))
            {
                print("UDP Thread has closed successfully - OnDestroy");
            }
            else
            {
                print("UDP Thread did not close in 100ms, abort - OnDestroy");
                receiveThread.Abort();
            }
            //receiveThread.Abort();
            ////receiveThread.Join();

            receiveThread = null;

        }

    }
}
