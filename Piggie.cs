using UnityEngine;

public class Piggie : MonoBehaviour
{
    [Header("Pig's Health")]
    // Salud máxima de los cerdos
    [SerializeField] private float _maxHealth = 3f;

    // Umbral de daño de los cerdos, para que solo les afecte si se supera
    [SerializeField] private float _damageThreshold = 0.2f;

    //Partículas de muerte del cerdo
    [SerializeField] private GameObject _piggieDeathParticles;

    //Sonido de muerte del cerdo
    [SerializeField] private AudioClip _deathClip;

    // Salud actual del cerdo
    private float _currentHealth;

    // Awake: asigna la salud actual del cerdo
    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    // DamagePiggie: decrementa la salud del cerdo en función del daño realizado
    public void DamagePiggie(float dammageAmmount)
    {
        _currentHealth -= dammageAmmount;

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    // Die: "Mata" al cerdo (borra su instancia)
    private void Die()
    {
        GameManager.instance.RemovePiggie(this);

        Instantiate(_piggieDeathParticles, transform.position, Quaternion.identity);

        AudioSource.PlayClipAtPoint(_deathClip, transform.position);
        Destroy(gameObject);
    }

    // OnCollisionEnter2D: si el cerdo recibe una colisión, se calcula la velocidad del impacto y,
    // en caso de superar el umbral de daño, decrementa la salud del cerdo
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude;
        if (impactVelocity > _damageThreshold)
        {
            DamagePiggie(impactVelocity);
        }
    }
}
