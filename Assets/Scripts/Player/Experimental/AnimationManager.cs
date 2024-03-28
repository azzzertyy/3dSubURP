using Unity.Netcode;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
{
    [SerializeField] private Animator playAnimator;

    
    public void UpdatePlayerClient(string newAnimation, bool isPlaying)
    {
        if(NetworkManager.Singleton.IsServer)
        {
            playAnimator.SetBool(newAnimation, isPlaying);
        }
        else
        {
            UpdatePlayerAnimationServerRpc(newAnimation, isPlaying);
            playAnimator.SetBool(newAnimation, isPlaying);
        }
    }

    [ServerRpc]
    public void UpdatePlayerAnimationServerRpc(string newAnimation, bool isPlaying)
    {
        Debug.Log("Updated player animation to: " + newAnimation + isPlaying);
        playAnimator.SetBool(newAnimation, isPlaying);
    }
}