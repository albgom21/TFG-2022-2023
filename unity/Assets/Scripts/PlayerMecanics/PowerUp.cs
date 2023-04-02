using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum powerUpTypes { 
        Gravity, 
        SlowMotion, 
        BadQuality 
    };

    [SerializeField] private powerUpTypes powerUpType;

    private PowerUpsManager powerUpsManager;
    private SpriteRenderer sprite;
    private BoxCollider2D collider;

    void Start()
    {
        powerUpsManager = GameManager.instance.getPowerUpsManager();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();

        powerUpsManager.addPowerUpInstance(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();

        if (pm != null) consumePowerUp();
    }

    public void consumePowerUp()
    { 
        sprite.enabled = false;
        collider.enabled = false;

        switch (powerUpType)
        {
            case powerUpTypes.Gravity:
                powerUpsManager.changeGravity();
                break;
            case powerUpTypes.SlowMotion:
                powerUpsManager.slowMotionOn(5.0f);
                break;
            case powerUpTypes.BadQuality:
                powerUpsManager.badQualityOn(5.0f);
                break;
            default:
                Debug.Log("KA PASAO POWER UP SIN TIPO O KHÉ");
                break;
        }
    }

    public void resetPowerUp() {
        sprite.enabled = true;
        collider.enabled = true;
    }
}
