using UnityEngine;

public class AngryBird : MonoBehaviour
{
    // Sonidos de colisión
    [SerializeField] private AudioClip[] _hitClips;

    // Componentes de físicas
    private Rigidbody2D _rb;
    private CircleCollider2D _circleCollider;

    // Bandera que indica si el pájaro ha sido lanzado
    private bool _hasBeenLaunched;
    // Bandera que indica la orientación del pájaro (cara)
    private bool _shouldFaceVelDirection;
    // Fuente de sonidos del pájaro
    private AudioSource _audioSource;

    // Awake: Preparación pájaro (componentes y fuente de audio)
    private void Awake(){
        _rb = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Start: Colisionador desconectado y estado de Rigidbody modificado
    private void Start()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _circleCollider.enabled = false;
    }

    // FixedUpdate: Si se lanza el pájaro y se encuentra en la dirección del lanzamiento, su transformada pasa a ser la velocidad lineal de su componente Rigidbody
    private void FixedUpdate()
    {
        if (_hasBeenLaunched && _shouldFaceVelDirection){
            transform.right = _rb.linearVelocity;
        }
    }

    // LaunchBird: Cambian las físicas del pájaro y se activa su colisionador.
    // Se añade una fuerza a su lanzamiento
    public void LaunchBird(Vector2 direction, float force)
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _circleCollider.enabled = true;

        _rb.AddForce(direction * force, ForceMode2D.Impulse);

        _hasBeenLaunched = true;
        _shouldFaceVelDirection = true;
    }

    // OnCollisionEnter2D: Al chocar con algo, deja de orientarse a una posición definida,
    // se reproduce un sonido y se destruye su instancia (NO DEL JUEGO)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _shouldFaceVelDirection = false;
        SoundManager.instance.PlayRandomClip(_hitClips, _audioSource);
        Destroy(this);
    }
}
