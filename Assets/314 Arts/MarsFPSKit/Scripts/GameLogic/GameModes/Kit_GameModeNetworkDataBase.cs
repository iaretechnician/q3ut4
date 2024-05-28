using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_GameModeNetworkDataBase : NetworkBehaviour
    {
        public override void OnStartServer()
        {
            Kit_IngameMain.instance.currentGameModeRuntimeData = this;
        }

        public override void OnStartClient()
        {
            Kit_IngameMain.instance.currentGameModeRuntimeData = this;
            Kit_IngameMain.instance.OnClientDataReadyToSetup();
        }
    }
}