using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuración de comportamiento")]
    [SerializeField] private float speed = 3.0f;

    [Header("Configuración de estadísticas")]
    [SerializeField] private int dmg = 1;
    [SerializeField] private int life = 1;
    public float score = 10f;

    [SerializeField] private Configuracion_General config;

    private void Awake()
    {
        GameObject gm = GameObject.FindWithTag("GameController");
        if (gm != null)
        {
            config = gm.GetComponent<Configuracion_General>();
        }
    }

    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (Configuracion_General.runner3D == false)
        {
            if (transform.position.y >= -6.0f)
            {
                transform.Translate(Vector3.down * speed * Time.deltaTime);
            }
            else
            {
                destroyMe();
            }
        }
        else
        {
            if (transform.position.z >= -6.0f)
            {
                transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            else
            {
                destroyMe();
            }
        }
    }

    private void Damage(int _dmg)
    {
        if (life > 0)
        {
            life -= _dmg;
            if (life <= 0)
            {
                destroyMe();
                giveScore(score);
            }
        }
        else if (life <= 0)
        {
            destroyMe();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(dmg);
                life--;
            }

            if (life <= 0)
            {
                destroyMe();
            }
        }
    }

    private void giveScore(float _score)
    {
        if (config != null)
        {
            config.puntos += _score;
        }
        else
        {
            Debug.Log("No hay un Script de configuración general asignado");
        }
    }

    private void destroyMe()
    {
        if (SpawnManager.activeEnemies.Count > 0)
        {
            for (int i = 0; i < SpawnManager.activeEnemies.Count; i++)
            {
                if (SpawnManager.activeEnemies[i].transform.position == this.gameObject.transform.position)
                {
                    SpawnManager.activeEnemies.Remove(SpawnManager.activeEnemies[i]);
                    Destroy(this.gameObject);
                    break;
                }
            }
        }
    }
}
