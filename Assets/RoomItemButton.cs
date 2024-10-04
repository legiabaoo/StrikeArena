using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string Roomname;
    public void OnButtonPressed()
    {
        Roomlist.Instance.JoinRoomByName(Roomname);
    }
}
