using UnityEngine;

class TuioToupie
{
  public enum State { Unknown = 0, New = 1, Update = 2, Removed = 3 };

  public State   state;
  public int     id;
  public Vector2 position;
  public GameObject gameobj;

  public TuioToupie(int _id, float _x, float _y)
  {
    state    = State.Unknown;
    id       = _id;
    position = new Vector2(_x, _y);
  }

  public override string ToString()
  {
    return "state "+ state.ToString() +" id "+ id +" pos "+ position.ToString() +" gameobj "+ gameobj;
  }
}
