using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;


//beacon locations are *100, x-500 and y-300
[System.Serializable]
public class UDPConnect : MonoBehaviour
{
    UdpClient client;
    byte[] bytesToSend;
    IPEndPoint remoteEndPoint;
    bool butPressed = false;

    // public List<HedgehogData> data;

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

        InvokeRepeating("GetUpdatedCoords", 0.5f, 1.0f);
    }

    void Update() {
        if(butPressed) {
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
            butPressed = false;
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
                bytesToSend = Encoding.ASCII.GetBytes("top right diagnol");
                break;
            case 2:
                bytesToSend = Encoding.ASCII.GetBytes("right");
                break;
            case 3:
                bytesToSend = Encoding.ASCII.GetBytes("bottom right diagnol");
                break;
            case 4:
                bytesToSend = Encoding.ASCII.GetBytes("bottom");
                break;
            case 5:
                bytesToSend = Encoding.ASCII.GetBytes("bottom left diagnol");
                break;
            case 6:
                bytesToSend = Encoding.ASCII.GetBytes("left");
                break;
            case 7:
                bytesToSend = Encoding.ASCII.GetBytes("top left diagnol");
                break;
            case 8:
                bytesToSend = Encoding.ASCII.GetBytes("turn counter clockwise");
                break;
            case 9:
                bytesToSend = Encoding.ASCII.GetBytes("turn clockwise");
                break;
            case 22:
                bytesToSend = Encoding.ASCII.GetBytes("TEST");
                break;
        }

        butPressed = true;
    }

    void GetUpdatedCoords()
    {
        //send update message to get updated coords
        try{
            bytesToSend = Encoding.ASCII.GetBytes("UPDATE");
            client.Send(bytesToSend, bytesToSend.Length);
            remoteEndPoint = new IPEndPoint(IPAddress.Any, 5500);
            byte[] receiveBytes = client.Receive(ref remoteEndPoint);
            string receivedString = Encoding.ASCII.GetString(receiveBytes);
            print("Message received from the server \n " + receivedString);
            // data = JsonUtility.FromJson<HedgehogData>(receivedString);
            // print(data.address);
            // print(data.coords); 
            // HedgehogData hh = HedgehogData.CreateFromJSON(receivedString);
            // print("coords = " + hh.coords);
        }catch(Exception e) {
            print("Exception thrown " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        //send message to stop server
        try{
            bytesToSend = Encoding.ASCII.GetBytes("QUIT");
            client.Send(bytesToSend, bytesToSend.Length);
        }catch(Exception e) {
            print("Exception thrown " + e.Message);
        }
        print("quitting");
    }
}

// [System.Serializable]
// public class HedgehogData
// {
//     public int count;
//     public Hedgehog[] hh;
 
//     public static HedgehogData CreateFromJSON(string jsonString)
//     {
//         return JsonUtility.FromJson<HedgehogData>(jsonString);
//     }
// }

// [System.Serializable]
// public class Hedgehog
// {
//     public int address;
//     public float[] coords;
// }