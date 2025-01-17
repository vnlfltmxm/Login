using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SignInController : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField ID_InputFiled;
    [SerializeField]
    private TMP_InputField Passward_InputFiled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushAcceptButton()
    {

        DBController.Instance.SignIn(ID_InputFiled.text, Passward_InputFiled.text);

        ID_InputFiled.text = string.Empty;
        Passward_InputFiled.text = string.Empty;
        this.gameObject.SetActive(false);
    }
}
