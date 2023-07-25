
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private string _playerName;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private int _count;

        private Dictionary<int, ShipController> _players = new Dictionary<int, ShipController>();
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();
            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.GetComponent<ShipController>().PlayerName = _playerName;
            _players.Add(conn.connectionId, player.GetComponent<ShipController>());
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler(100, ReceiveName);
        }
        
        public class MessageLogin : MessageBase
        {
            public string Login;

            public override void Deserialize(NetworkReader reader)
            {
                Login = reader.ReadString();
            }

            public override void Serialize(NetworkWriter writer)
            {
                writer.Write(Login);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            MessageLogin _login = new MessageLogin();
            _login.Login = _inputField.text;
            conn.Send(100, _login);
        }

        public void ReceiveName(NetworkMessage networkMessage)
        {
            _players[networkMessage.conn.connectionId].PlayerName = networkMessage.reader.ReadString();
            _players[networkMessage.conn.connectionId].gameObject.name =
                _players[networkMessage.conn.connectionId].PlayerName;
            Debug.Log(_players[networkMessage.conn.connectionId]);
        }
    }
}
