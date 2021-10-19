﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using protocol.cs_enum;
using protocol.cs_theircraft;
using TMPro;

public class ChatPanel : MonoBehaviour
{
    List<Transform> ItemList = new List<Transform>();
    Transform itemUnit;
    GridLayoutGroup grid;
    GridLayoutGroup floatingGrid;
    TMP_InputField inputField;
    GameObject container;
    RectTransform scrollview;
    RectTransform floatingScrollview;

    static Queue<string> _message = new Queue<string>();

    public static readonly string ErrorCode = "<color=#FF5555>";
    public static readonly string HideCode = "[hide]";

    static ChatPanel instance;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (SettingsPanel.ShowLog == 0)
        {
            return;
        }

        if (type == LogType.Log)
        {
            if (!logString.StartsWith(HideCode))
            {
                AddLine("<Log>" + logString.Replace('\n', ' '));
            }
        } else if (type == LogType.Error)
        {
            AddLine(ErrorCode + "<Error>" + logString.Replace('\n', ' '));
        }
    }

    public static void ShowChatPanel()
    {
        if (instance == null)
        {
            instance = UISystem.InstantiateUI("ChatPanel").GetComponent<ChatPanel>();
        }
    }

    static bool initialized;

    public static void AddLine(string content)
    {
        if (initialized)
        {
            instance.AddLineItem(content);
        }
        else
        {
            _message.Enqueue(content);
        }
    }

    // Use this for initialization
    void Start ()
    {
        initialized = true;
        NetworkManager.Register(ENUM_CMD.CS_MESSAGE_NOTIFY, OnMessageNotify);

        scrollview = transform.Find("container/Scroll View").GetComponent<RectTransform>();
        floatingScrollview = transform.Find("container/floatingScrollview").GetComponent<RectTransform>();
        container = transform.Find("container").gameObject;
        itemUnit = transform.Find("container/Scroll View/Content/chat_item");
        itemUnit.gameObject.SetActive(false);
        grid = transform.Find("container/Scroll View/Content").GetComponent<GridLayoutGroup>();
        floatingGrid = transform.Find("container/floatingScrollview/Content").GetComponent<GridLayoutGroup>();

        inputField = transform.Find("container/InputField").GetComponent<TMP_InputField>();
        inputField.gameObject.SetActive(false);

        while (_message.Count > 0)
        {
            string content = _message.Dequeue();
            AddLineItem(content);
        }

        scrollview.gameObject.SetActive(false);
        floatingScrollview.gameObject.SetActive(true);
    }

    void ShowInput()
    {
        InputManager.enabled = false;
        scrollview.gameObject.SetActive(true);
        floatingScrollview.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.ActivateInputField();
        PlayerController.LockCursor(false);
    }

    void HideInput()
    {
        InputManager.enabled = true;
        inputField.DeactivateInputField();
        inputField.gameObject.SetActive(false);
        PlayerController.LockCursor(true);
        scrollview.gameObject.SetActive(false);
        floatingScrollview.gameObject.SetActive(true);
    }
    
    void ProcessInput()
    {
        if (scrollview.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (inputField.text != "")
                {
                    OnClickSendButton();
                }
                else
                {
                    HideInput();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideInput();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                // last command
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                inputField.text = "";
                ShowInput();
            }
            else if (Input.GetKeyDown(KeyCode.Slash))
            {
                ShowInput();
                inputField.text = "/";
                inputField.caretPosition = inputField.text.Length;
            }
        }
    }

    int width = 340;
    int height = 9;
    
    void Update()
    {
        ProcessInput();
        if (floatingScrollview.sizeDelta.y != height * floatingGrid.transform.childCount)
        {
            floatingScrollview.sizeDelta = new Vector2(width, height * floatingGrid.transform.childCount);
        }
    }

    void OnDestroy()
    {
        initialized = false;
        instance = null;
        ItemList.Clear();
    }

    void AddLineItem(string content)
    {
        Transform item = Instantiate(itemUnit);
        item.gameObject.SetActive(true);
        item.SetParent(itemUnit.parent);
        item.localScale = Vector3.one;
        TextMeshProUGUI text = item.GetComponent<TextMeshProUGUI>();
        text.text = content;
        if (content.StartsWith("Saved screenshot as"))
        {
            item.gameObject.AddComponent<ScreenshotClickHelper>();
        }

        ItemList.Add(item);
        scrollview.sizeDelta = new Vector2(width, height * ItemList.Count);

        Transform floatingItem = Instantiate(item);
        floatingItem.SetParent(floatingGrid.transform);
        floatingItem.localScale = Vector3.one;
        floatingScrollview.sizeDelta = new Vector2(width, height * floatingGrid.transform.childCount);
        Destroy(floatingItem.gameObject, 10);
    }

    void OnClickSendButton()
    {
        if (inputField.text != "")
        {
            if (inputField.text.StartsWith("/"))
            {
                string text = inputField.text;

                inputField.text = "";
                inputField.ActivateInputField();
                HideInput();

                GM.Process(text);
            }
            else
                SendMessageReq(inputField.text);
        }
        else
        {
            FastTips.Show(1);
        }
    }

    void SendMessageReq(string content)
    {
        CSSendMessageReq req = new CSSendMessageReq
        {
            Content = content
        };
        NetworkManager.SendPkgToServer(ENUM_CMD.CS_SEND_MESSAGE_REQ, req, OnSendMessageRes);
    }

    void OnSendMessageRes(object data)
    {
        CSSendMessageRes rsp = NetworkManager.Deserialize<CSSendMessageRes>(data);
        //Debug.Log("OnSendMessageRes,retCode=" + res.RetCode);
        if (rsp.RetCode == 0)
        {
            inputField.text = "";
            inputField.ActivateInputField();
            HideInput();
        }
        else
        {
            FastTips.Show(rsp.RetCode);
        }
    }
    static void OnMessageNotify(object data)
    {
        CSMessageNotify notify = NetworkManager.Deserialize<CSMessageNotify>(data);
        //Debug.Log($"OnMessageNotify,name={notify.Name},content={notify.Content}");
        AddLine("<" + notify.Name + "> " + notify.Content);
    }
}
