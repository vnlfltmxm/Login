using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField IDText;
    [SerializeField]
    private TMP_InputField PasswardText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushLodinButton()
    {
        DBController.Instance.Login(IDText.text, PasswardText.text);
    }
}
