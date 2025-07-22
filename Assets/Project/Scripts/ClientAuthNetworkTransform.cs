using Unity.Netcode.Components;
using UnityEngine;

public class ClientAuthNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return base.OnIsServerAuthoritative();
    }
}
