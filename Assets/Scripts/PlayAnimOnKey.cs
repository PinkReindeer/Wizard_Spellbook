using UnityEngine;
using UnityEngine.InputSystem;

public class PlayAnimOnKey : MonoBehaviour
{
    public GameObject projectile;
    public ParticleSystem particleSystem;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            projectile.SetActive(true);
        }

        if (projectile.activeSelf && !particleSystem.IsAlive())
        {
            projectile.SetActive(false);
        }
    }
}
