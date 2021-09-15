using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unfold : MonoBehaviour
{
    public GameObject[] menus;
    public void UnfoldMenuIndex(int index)
    {
        menus[index].SetActive(!menus[index].activeSelf);
    }
}
