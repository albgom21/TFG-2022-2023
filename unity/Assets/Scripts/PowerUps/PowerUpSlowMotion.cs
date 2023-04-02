using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSlowMotion : MonoBehaviour
{
    [SerializeField] private float timeScaleWhenEnabled;
    private bool slowMotion;
    private float timer;

    private void Start()
    {
        slowMotion = false;
        timer = 0.0f;
    }
    private void Update()
    {
        if (slowMotion)
        {
            timer += Time.deltaTime;
            if (timer >= 5.0f)
            {
                slowMotion = false;
                updateTimeScale(1.0f);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement pm = collision.gameObject.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            slowMotion = true;
            updateTimeScale(timeScaleWhenEnabled);
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void updateTimeScale(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
}
