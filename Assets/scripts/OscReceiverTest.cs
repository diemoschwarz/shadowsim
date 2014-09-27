using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Simple OSC test communication script
/// </summary>
//[AddComponentMenu("Scripts/OscSenderReceiver")]
public class OscReceiverTest : MonoBehaviour
{
    private Osc oscHandler;
    
    public string remoteIp;
    public int sendToPort;
    public int listenerPort;
    public GameObject toupie;
    private PosFromOsc toupiepos;

    ~OscReceiverTest()
    {
        if (oscHandler != null)
        {            
            oscHandler.Cancel();
        }

        // speed up finalization
        oscHandler = null;
        System.GC.Collect();
    }

    public void SendOSC(string address, params object[] wordList)
    { 
      OscMessage oscM = new OscMessage();
      oscM.Address = address;
      
      for (int i = 0; i < wordList.Length; i++) 
	{ 
	  oscM.Values.Add(wordList[i]);
	} 
		
      oscHandler.Send(oscM);
    }

    void OnDisable()
    {
        // close OSC UDP socket
        Debug.Log("closing OSC UDP socket in OnDisable");
        oscHandler.Cancel();
        oscHandler = null;
    }

    /// <summary>
    /// Start is called just before any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        UDPPacketIO udp = GetComponent<UDPPacketIO>();
        udp.init(remoteIp, sendToPort, listenerPort);
        
	oscHandler = GetComponent<Osc>();
        oscHandler.init(udp);
        
        oscHandler.SetAddressHandler("/tuio/2Dcur", Example);
	toupiepos = toupie.GetComponent("PosFromOsc") as PosFromOsc;
    }

    public void Example (OscMessage m)
    {
      string args = "";

      for (int i = 0; i < m.Values.Count; i++)
	args += m.Values[i].ToString() + " ";

      //Debug.Log("---> OSC message "+ m.Values.Count +" args: "+ m.Address +" "+ args);

      if (m.Values[0].ToString() == "set")
      {
//	int seq = (int) m.Values[1];
	float x = (float) m.Values[2];
	float y = (float) m.Values[3];
	//Debug.Log("set "+ x +", "+ y);

	float scale = 10;
	toupiepos.x = x * scale;
	toupiepos.y = y * scale;
      }
    }
}
