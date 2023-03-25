using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelterPage : Page
{
    [Space]
    [SerializeField] public GameObject Shelter1;
    [SerializeField] public GameObject Shelter2;
    [SerializeField] public GameObject Shelter3;
    [SerializeField] public GameObject Shelter4;

    private void OnEnable()
    {
        Shelter1.SetActive(true);
        Shelter2.SetActive(false);
        Shelter3.SetActive(false);
        Shelter4.SetActive(false);
    }
}
