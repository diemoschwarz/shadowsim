using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Simple OSC test communication script
/// </summary>
//[AddComponentMenu("Scripts/OscSenderReceiver")]
public class MolfReceiver : MonoBehaviour
{
    public string remoteIp;
    public int sendToPort;
    public int listenerPort;

    public Transform toupieModel;	// prefab
    public int numFakeToupies;

    private Osc oscHandler;
    private Hashtable toupies;

    ~MolfReceiver ()
    {
        if (oscHandler != null)
        {            
            oscHandler.Cancel();
        }

        // speed up finalization
        oscHandler = null;
        System.GC.Collect();
    }

    void OnDisable()
    {
        // close OSC UDP socket
        Debug.Log("closing OSC UDP socket in OnDisable");
        oscHandler.Cancel();
        oscHandler = null;
    }

    void Start()
    {
	toupies = new Hashtable();

        UDPPacketIO udp = GetComponent<UDPPacketIO>();
        udp.init(remoteIp, sendToPort, listenerPort);
        
	oscHandler = GetComponent<Osc>();
        oscHandler.init(udp);
        
        oscHandler.SetAddressHandler("/tuio/2Dcur", cursor_handler);
    }

    public void cursor_handler (OscMessage m)
    {
      /*      string args = "";
      for (int i = 0; i < m.Values.Count; i++)
	args += m.Values[i].ToString() + " ";

      Debug.Log("---> OSC message "+ m.Values.Count +" args: "+ m.Address +" "+ args);
      */

      switch (m.Values[0].ToString())
      {
        case "set":
	  int seq = (int) m.Values[1];
	  float x = (float) m.Values[2];
	  float y = (float) m.Values[3];
	  //Debug.Log("set "+ seq +" "+ x +", "+ y +" count "+ toupies.Count);

	  TuioToupie toupie;

	  if (toupies.Contains(seq))
	  {
	    toupie = toupies[seq] as TuioToupie;
	    toupie.state   = TuioToupie.State.Update;
	    toupie.position = new Vector2(x, y);
	  }
	  else
	  {
	    toupie = new TuioToupie(seq, x, y);
	    toupie.state = TuioToupie.State.New;
	    toupies.Add(seq, toupie);
	  }
	break;
	
        case "alive":
	  // copy to hash, go through known points to find removed points
	  Hashtable alive = new Hashtable();
	  for (int i = 1; i < m.Values.Count; i++)
	    alive.Add((int) m.Values[i], 1);

	  foreach (DictionaryEntry t in toupies)
	  {
	    int id = (int) t.Key;
	    if (!alive.Contains(id))
	      (toupies[id] as TuioToupie).state = TuioToupie.State.Removed;
	  }
	break;
      }
    }

  void Update ()
  {
    for (int seq = 0; seq < numFakeToupies; seq++)
    {
      TuioToupie toupie;
      float t = Time.time;
      float x =  Mathf.Sin(t * 2 * Mathf.PI / (seq + 1)) * 0.5f + 0.5f;
      float y =  Mathf.Sin(t * 2 * Mathf.PI / (seq + 1.73f)) * 0.5f + 0.5f;

      if (toupies.Contains(seq))
      {
	toupie = toupies[seq] as TuioToupie;
	toupie.state   = TuioToupie.State.Update;
	toupie.position = new Vector2(x, y);
      }
      else
      {
	toupie = new TuioToupie(seq, x, y);
	toupie.state = TuioToupie.State.New;
	toupies.Add(seq, toupie);
      }
    }

    // Debug.Log("MolfReceiver::Update "+ toupies.Count +" toupies");
    ArrayList killlist = new ArrayList();

    // go through toupies hash, update objects
    foreach (DictionaryEntry t in toupies)
    {
      TuioToupie toupie = t.Value as TuioToupie;
      Vector3 pos;

      switch (toupie.state)
      {
      case TuioToupie.State.New:
	// create game object
	pos = new Vector3(toupie.position.x, 0, toupie.position.y) * 10;

	toupie.gameobj = (GameObject) Instantiate(toupieModel, pos, Quaternion.identity);
	toupie.state = TuioToupie.State.Update;

	Debug.Log("MolfReceiver::Update new "+ toupie +" "+ toupie.ToString());
      break;
	
      case TuioToupie.State.Update:
	if (!toupie.gameobj)
	{
	  //Debug.Log("update: empty gameobj "+ toupie);
	  break;
	}
	pos = toupie.gameobj.transform.position;
	pos.x = toupie.position.x * 20;
	pos.z = toupie.position.y * 10;
	toupie.gameobj.transform.position = pos;
      break;

      case TuioToupie.State.Removed:
 	Debug.Log("MolfReceiver::Update removed "+ toupie +" "+ toupie.ToString());

	// kill gameobject
	Destroy(toupie.gameobj);

	// can't remove from hash directly:
	// toupies.Remove(t.Key); throws InvalidOperationException: Hashtable.Enumerator: snapshot out of sync.
	killlist.Add(t.Key);
      break;
      }
    }

    // do the killing
    foreach (object k in killlist)
      toupies.Remove(k);
  }
}

/** EMACS **
 * Local variables:
 * mode: c-sharp
 * c-style: "strustroup"
 * c-basic-offset:2
 * End:
 */
