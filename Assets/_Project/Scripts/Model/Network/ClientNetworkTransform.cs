using Unity.Netcode.Components;
using UnityEngine;

namespace TW
{
    public enum AuthorityMode
    {
        Server,
        Client
    }

    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        [SerializeField]
        public AuthorityMode mode = AuthorityMode.Client;

        protected override bool OnIsServerAuthoritative() => mode == AuthorityMode.Server;

        public void SetServerAuthorative(AuthorityMode authorityMode)
        {
            mode = authorityMode;
        }
    }
}
