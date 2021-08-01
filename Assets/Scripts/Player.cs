using UnityEngine;

public class Player : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)] public PlayerController playerController;
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            playerController.SendLevelLose();
            playerController.SendEndLevel();
            playerController.StopRunning();
        }
    }
}
