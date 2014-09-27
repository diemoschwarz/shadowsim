using UnityEngine;

public class PosFromOsc : MonoBehaviour
{
  public float angle;
  public float frequency;

  public float x;
  public float y;

  private Vector3 scale;

  void Start ()
  {
    scale = transform.localScale;
  }

  void Update()
  {
    Vector3 pos = transform.position;
    pos.x = x;
    pos.z = y;
    transform.position = pos;

    float t = Time.time;
    float a = Mathf.Sin(t * 2 * Mathf.PI * frequency) * angle;
    float b = Mathf.Cos(t * 2 * Mathf.PI * frequency) * angle;

    // rot about centre
    Quaternion rot = Quaternion.Euler(a, 0f, b);
    transform.rotation = rot;

    //    transform.RotateAround(pos - new Vector3(0, -1, 0), Vector3.forward, a);

    // simulate missing angular spotlight: make shadows bigger when close
    //Debug.Log(y +" "+ ((10 - y) + 1));
    transform.localScale = scale * ((10 - y) + 1);
  }
}