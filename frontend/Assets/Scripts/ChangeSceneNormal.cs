// Start is called before the first frame update
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // This is so that it should find the Text component
using UnityEngine.Events; // This is so that you can extend the pointer handlers
using UnityEngine.EventSystems;
public class ChangeSceneNormal : MonoBehaviour
{
    public void changeScene(string scenename)
    { 
        SceneManager.LoadScene(scenename);
    }
    public void lightFont(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.white;
    }
    public void darkFont(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.black;
    }

    void Update()
    {
    }
}
