using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum powerUpTypes { 
        Gravity, 
        SlowMotion, 
        LowRes 
    };

    [SerializeField] private powerUpTypes powerUpType;

    private PowerUpsManager powerUpsManager;
    private SpriteRenderer sprite;
    private BoxCollider2D collider;

    void Start()
    {
        powerUpsManager = GameManager.instance.GetPowerUpsManager();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();

        powerUpsManager.AddPowerUpInstance(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();

        if (pm != null) ConsumePowerUp();
    }

    public void ConsumePowerUp()
    { 
        sprite.enabled = false;
        collider.enabled = false;

        switch (powerUpType)
        {
            case powerUpTypes.Gravity:
                powerUpsManager.ChangeGravity();
                break;
            case powerUpTypes.SlowMotion:
                powerUpsManager.SlowMotionOn(5.0f); // HACER REFACTOR DE ESTA VARIABLE 
                break;
            case powerUpTypes.LowRes:
                powerUpsManager.LowResOn(5.0f);
                break;
            default:
                Debug.Log("KA PASAO POWER UP SIN TIPO O KHÉ");
                break;
        }
    }

    public void ResetPowerUp() {
        sprite.enabled = true;
        collider.enabled = true;
    }
}
