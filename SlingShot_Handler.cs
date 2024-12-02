using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShot_Handler : MonoBehaviour
{
    [Header("Line Renderers")]
    // Renderizadores de líneas
    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform References")]
    // Posiciones de las gomas del tirachinas
    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;
    [SerializeField] private Transform _elasticTransform;

    [Header("Slinghshot Stats")]
    // Distancia máxima de elasticidad del tirachinas
    [SerializeField] private float _maxDistance = 3.5f;

    // Fuerza de lanzamiento
    [SerializeField] private float _shotForce = 9f;

    // Tiempo necesario entre pájaros
    [SerializeField] private float _timeBetweenBirdRespawns = 2f;

    // Constante utilizada para animar el tirachinas
    [SerializeField] private float _elasticDivider = 1.2f;

    // Animación del tirachinas (modificable en Unity)
    [SerializeField] private AnimationCurve _elasticCurve;

    // Tiempo máximo de animación de gomas del tirachinas
    [SerializeField] private float _maxAnimationTime = 1f;

    [Header("Scripts")]
    // Script de zona de detección de tirachinas
    [SerializeField] private SlingShot_Area _slingShotArea;

    // Gestor de la cámara
    [SerializeField] private CameraManager _cameraManager;

    [Header("Bird")]
    // "Prefab" del pájaro
    [SerializeField] private AngryBird _angryBirdPrefab;
    // Offset del pájaro con respecto al tirachinas
    [SerializeField] private float _angryBirdPositionOffset = 2f;

    [Header("Sounds")]
    // Sonido de tensión de gomas
    [SerializeField] private AudioClip _elasticPulledClip;
    // Sonido de lanzamiento del pájaro
    [SerializeField] private AudioClip[] _elasticReleasedClips;

    // Posiciones de las líneas del tirachinas
    private Vector2 _slingShotLinesPosition;

    // Dirección
    private Vector2 _direction;
    private Vector2 _directionNormalized;

    // Bandera para comprobar si se hace clic en el area esperada (lanzamiento)
    private bool _clickedWithinArea;
    // Bandera para comprobar si existe algún pájaro en el tirachinas
    private bool _birdOnSlingshot;

    // Pájaro instanciado
    private AngryBird _spawnedAngryBird;

    // Fuente de sonido (tirachinas)
    private AudioSource _audioSource;

    // Awake: Preparación tirachinas (fuente de audio y renderizadores)
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;

        SpawnAngryBird();
    }

    // Update: Función ejecutada cada frame
    private void Update()
    {
        // Si se pulsa en el área con clic izquierdo, suena el sonido de la goma y se actualiza la posición de la cámara
        if (InputManager.WasLeftMouseButtonPressed && _slingShotArea.IsWithinSlingshotArea())
        {
            _clickedWithinArea = true;

            if (_birdOnSlingshot)
            {
                SoundManager.instance.PlayClip(_elasticPulledClip, _audioSource);
                _cameraManager.SwitchToFollowCam(_spawnedAngryBird.transform);
            }
        }

        // Si se mantiene el clic izquierdo, se ha pulsado inicialmente en el área y hay un pájaro en el tirachinas,
        // se dibuja su posición y la orientación del pájaro
        if (InputManager.IsLeftMousePressed && _clickedWithinArea && _birdOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotateAngryBird();
        }

        // Si se levanta el clic izquierdo y se cumplen las condiciones anteriores, se lanza el pájaro,
        // se reproduce un sonido y se renderizan las nuevas líneas del tirachinas. En caso de quedar pájaros,
        // se instancian en el tirachinas
        if (InputManager.WasLeftMouseButtonReleased && _birdOnSlingshot && _clickedWithinArea)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                _clickedWithinArea = false;
                _birdOnSlingshot = false;

                _spawnedAngryBird.LaunchBird(_direction, _shotForce);

                SoundManager.instance.PlayRandomClip(_elasticReleasedClips, _audioSource);

                GameManager.instance.UseShot();
                //Setlines(_centerPosition.position); -- Utilizada antes de la creación de la función AnimateSlingShot
                AnimateSlingShot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnAngryBirdAfterTime());
                }
            }
        }
    }

    #region SlingShot Methods

    // DrawSlingShot: Renderizado del estado del tirachinas
    private void DrawSlingShot()
    {
        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(InputManager.MousePosition);

        _slingShotLinesPosition = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position, _maxDistance);

        Setlines(_slingShotLinesPosition);

        _direction = (Vector2)_centerPosition.position - _slingShotLinesPosition;
        _directionNormalized = _direction.normalized;
    }

    // Setlines: Posicionamiento de líneas por defecto en el tirachinas
    private void Setlines(Vector2 position)
    {
        if (!_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }

    #endregion

    #region Angry Bird Methods

    // SpawnAngryBird: Instanciación del pájaro
    private void SpawnAngryBird()
    {
        _elasticTransform.DOComplete();
        Setlines(_idlePosition.position);

        Vector2 dir = (_centerPosition.position - _idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)_idlePosition.position + dir * _angryBirdPositionOffset;

        _spawnedAngryBird = Instantiate(_angryBirdPrefab, spawnPosition, Quaternion.identity);
        _spawnedAngryBird.transform.right = dir;

        _birdOnSlingshot = true;
    }

    // PositionAndRotateAngryBird: Posición y rotación del pájaro
    private void PositionAndRotateAngryBird()
    {
        _spawnedAngryBird.transform.position = _slingShotLinesPosition + _directionNormalized * _angryBirdPositionOffset;
        _spawnedAngryBird.transform.right = _directionNormalized;
    }

    // SpawnAngryBirdAfterTime: Reinstanciación de un pájaro en el tirachinas tras un tiempo
    private IEnumerator SpawnAngryBirdAfterTime()
    {
        yield return new WaitForSeconds(_timeBetweenBirdRespawns);

        SpawnAngryBird();

        _cameraManager.SwitchToIdleCam();
    }

    #endregion

    #region Animate SlingShot

    // AnimateSlingShot: Animación del tirachinas
    private void AnimateSlingShot()
    {
        _elasticTransform.position = _leftLineRenderer.GetPosition(0);

        float dist = Vector2.Distance(_elasticTransform.position, _centerPosition.position);

        float time = dist / _elasticDivider;

        _elasticTransform.DOMove(_centerPosition.position, time).SetEase(_elasticCurve);
        StartCoroutine(AnimateSlingShotLines(_elasticTransform, time));
    }

    // AnimateSlingShotLines: Corrutina de animación de gomas del tirachinas
    private IEnumerator AnimateSlingShotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time && elapsedTime < _maxAnimationTime)
        {
            elapsedTime += Time.deltaTime;

            Setlines(trans.position);

            yield return null;
        }
    }

    #endregion
}
