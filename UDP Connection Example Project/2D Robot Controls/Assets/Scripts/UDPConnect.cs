using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System;


//beacon locations are *100, x-500 and y+230 /2
//moving beacons are ( /10*len ) * 100, x-500 and y+230 /2
[System.Serializable]
public class UDPConnect : MonoBehaviour
{
    private UdpClient client;
    private byte[] bytesToSend;
    private IPEndPoint remoteEndPoint;
    private bool butPressed = false;
    private Transform canvas;

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

        canvas = GameObject.FindWithTag("Canvas").transform;

        InvokeRepeating("GetUpdatedCoords", 0.25f, 0.25f);
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
            HedgehogData hh = HedgehogData.CreateFromJSON(receivedString);

            //update x and y coord
            // print($"coords was {hh.coords[0]} and {hh.coords[1]}");
            hh.coords[0] = (hh.coords[0] / 1000f) * 100; //x coordinate
            hh.coords[1] = (hh.coords[1] / 1000f) * 100; //y coordinate
            // print($"coords is actually {hh.coords[0]} and {hh.coords[1]}");

            MoveHedgeHog(hh.address, hh.coords);
            //hh.address and hh.coords hold the address and coordinates updated, so now can move the circles
        }catch(Exception e) {
            print("Exception thrown " + e.Message);
        }
    }

    void MoveHedgeHog(int address, float[] coords) {
        Transform hh_sprite = GameObject.FindWithTag(address.ToString()).transform;
        //update local position
        hh_sprite.localPosition = new Vector3(coords[0], coords[1]*-1, 0);
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

[System.Serializable]
public class HedgehogData
{
    public int address;
    public float[] coords;

    public static HedgehogData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<HedgehogData>(jsonString);
    }
}