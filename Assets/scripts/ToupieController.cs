using UnityEngine;

public class ToupieController : MonoBehaviour
{
  public float meanAngle;
  public float meanFrequency;

  public float effectiveAngle;
  public float effectiveFrequency;

  public float scaleFactor = 1;
  private Vector3 scale;

  void Start ()
  {
    effectiveAngle     = meanAngle     + Random.Range(-10, 10);
    effectiveFrequency = meanFrequency * Random.Range(0.5f, 2);
    scale = transform.localScale;
  }

  void Update()
  {
    // position updated by ToupieFactory

    float t = Time.time;
    float a = Mathf.Sin(t * 2 * Mathf.PI * effectiveFrequency) * effectiveAngle;
    float b = Mathf.Cos(t * 2 * Mathf.PI * effectiveFrequency) * effectiveAngle;

    // rot about centre
    Quaternion rot = Quaternion.Euler(a, 0f, b);
    transform.rotation = rot;

    //    transform.RotateAround(pos - new Vector3(0, -1, 0), Vector3.forward, a);

    // simulate missing angular spotlight: make shadows bigger when close
    float y = transform.position.z;
    transform.localScale = scale * ((10 - y) + 1) * scaleFactor;
    //Debug.Log(y +" "+ ((10 - y) + 1));
  }
}
