using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private int isCreator;

    public GameObject VoiceObject;

    public GameObject QuitPanel;
    public GameObject ChatPanel;

    public TMP_Text[] ChatText;
    public TMP_InputField ChatInput;
    public PhotonView PV;

    public bool isChatPanelOn = false;
    public bool isQuitPanelOn = false;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManger Start");

        isCreator = PlayerPrefs.GetInt("isCreator");
        Debug.Log("isCreator");
        Debug.Log(isCreator);
        if (PlayerPrefs.GetInt("isVoice") == 0)
        {
            VoiceObject.SetActive(false);
        }

        Debug.Log("isCreator");
        Debug.Log(isCreator);
        if (isCreator == 0)
        {
            GameObject.Find("WorldEdit").SetActive(false);
            CreateHuman();
        }
        else if (isCreator == 1)
        {
            GameObject.Find("CameraControl").GetComponent<CameraControl>().editCameraOn();
        }
        //PhotonNetwork.IsMessageQueueRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isChatPanelOn && !isQuitPanelOn)
            {
                isChatPanelOn = true;
                ChatPanel.SetActive(true);
            }
            else
            {
                Send();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitPanel.SetActive(true);
        }
    }

    #region QUIT_GAME
    public void QuitPanelOff()
    {
        isQuitPanelOn = false;
        QuitPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    public void CreateHuman()
    {
        // Lecture Room : 12, 0, 349
        // Art Gallery : -4, -4, 8
        // 

        ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        string sceneName = (string)(ht["sceneName"]);

        if(sceneName == "Winter")
        {
            PhotonNetwork.Instantiate("human", new Vector3(688f, 10f, 692f), Quaternion.identity);
        }
        else if(sceneName == "Nature")
        {
            PhotonNetwork.Instantiate("human", new Vector3(688f, 10f, 692f), Quaternion.identity);
        }
        else if(sceneName == "ArtGallery")
        {
            PhotonNetwork.Instantiate("human", new Vector3(-4f, -4f, 8f), Quaternion.identity);
        }
        else if(sceneName == "LectureRoom")
        {
            PhotonNetwork.Instantiate("human", new Vector3(12f, 0f, 349f), Quaternion.identity);
        }

        GameObject.Find("CameraControl").GetComponent<CameraControl>().mainCameraOn();
    }

    #region CHAT

    public void Send()
    {
        if (ChatInput.text.ToString() == "")
        {
            isChatPanelOn = false;
            ChatPanel.SetActive(false); 
        }
        else
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }
    }

    [PunRPC] // RPC�� �÷��̾ �����ִ� �� ��� �ο����� �����Ѵ�
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
        ChatPanel.SetActive(true);

        Invoke("wait5sec", 5f);
    }

    void wait5sec()
    {
        ChatPanel.SetActive(false);
        isChatPanelOn = false;
    }
    #endregion


}
