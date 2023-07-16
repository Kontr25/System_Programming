using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviour
{
    [SerializeField] private Button buttonStartServer;
    [SerializeField] private Button buttonShutDownServer;
    [SerializeField] private Button buttonConnectClient;
    [SerializeField] private Button buttonDisconnectClient;
    [SerializeField] private Button buttonSendMessage;
    [SerializeField] private Button buttonOK;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextField textField;
    [SerializeField] private Server server;
    [SerializeField] private Client client;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject nameWindow;
    
    private void Start()
    {
        buttonStartServer.onClick.AddListener(() => StartServer());
        buttonShutDownServer.onClick.AddListener(() => ShutDownServer());
        buttonConnectClient.onClick.AddListener(() => Connect());
        buttonDisconnectClient.onClick.AddListener(() => Disconnect());
        buttonSendMessage.onClick.AddListener(() => SendMessage());
        buttonOK.onClick.AddListener(() => DisableNameInputField());
        client.onMessageReceive += ReceiveMessage;
    }

    private void OnDestroy()
    {
        buttonStartServer.onClick.RemoveListener(() => StartServer());
        buttonShutDownServer.onClick.RemoveListener(() => ShutDownServer());
        buttonConnectClient.onClick.RemoveListener(() => Connect());
        buttonDisconnectClient.onClick.RemoveListener(() => Disconnect());
        buttonSendMessage.onClick.RemoveListener(() => SendMessage());
        buttonOK.onClick.RemoveListener(() => DisableNameInputField());
        client.onMessageReceive -= ReceiveMessage;
    }

    private void StartServer()
    {
        server.StartServer();
    }
    private void ShutDownServer()
    {
        server.ShutDownServer();
    }
    private void Connect()
    {
        client.Connect();
    }
    private void Disconnect()
    {
        client.Disconnect();
    }
    private void SendMessage()
    {
        client.SendMessage(inputField.text);
        inputField.text = "";
    }
    public void ReceiveMessage(object message)
    {
        textField.ReceiveMessage(message);
    }

    private void DisableNameInputField()
    {
        if(nameInputField.text == "") return;
        client.SendMessage(nameInputField.text);
        nameWindow.SetActive(false);
        inputField.text = "";
    }
}