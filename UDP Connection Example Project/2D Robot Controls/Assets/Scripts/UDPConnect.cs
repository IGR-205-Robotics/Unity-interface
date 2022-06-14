using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;

public class UDPConnect : MonoBehaviour
{
    UdpClient client;
    byte[] bytesToSend;
    IPEndPoint remoteEndPoint;
    bool update = true;

    void Start()
    {
        client = new UdpClient(5600);
        try
        {
            client.Connect("127.0.0.1", 5500);
            bytesToSend = Encoding.ASCII.GetBytes("TEST");
            client.Send(bytesToSend, bytesToSend.Length);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message received from the server \n " + receivedString);
        }
        catch(Exception e)
        {
            print("Exception thrown " + e.Message);
        }
    }

    void Update() {
        if(update) {
            //send the new bytes to server
            try{
                client.Send(bytesToSend, bytesToSend.Length);
                remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
                byte[] receiveBytes = client.Receive(ref remoteEndPoint);
                string receivedString = Encoding.ASCII.GetString(receiveBytes);
                print("Message received from the server \n " + receivedString);
            }catch(Exception e) {
                print("Exception thrown " + e.Message);
            }
            update = false;
        }
    }

    public void SendDirection(int dir) {
        //int : dir
        /**
        0: top
        1: left
        **/

        switch(dir) {
            case 0:
                bytesToSend = Encoding.ASCII.GetBytes("top");
                break;
            case 1:
                bytesToSend = Encoding.ASCII.GetBytes("left");
                break;
            case 2:
                bytesToSend = Encoding.ASCII.GetBytes("TEST");
                break;
        }

        update = true;
    }
}