using System;
using FishNet;
using FishNet.Object;
using UnityEngine;

namespace Core.Scenes
{
    public class TestTrigger : NetworkBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!IsOwner) return; // Только владелец может сообщить

            Server_NotifyTriggerEntered(col.gameObject.name); // или передавай нужные ID
        }

        [ServerRpc]
        private void Server_NotifyTriggerEntered(string targetName)
        {
            // действия на сервере
        }
    }
}