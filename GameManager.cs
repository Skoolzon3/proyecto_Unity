using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Número máximo de lanzamientos
    public int MaxNumberOfShots = 3;
    // Tiempo de espera hasta dar por perdida una partida
    [SerializeField] private float _secondsToWaitBeforeDeathCheck = 6f;
    // Objeto de reinicio de escenario
    [SerializeField] private GameObject _restartScreenObject;
    // Script del tirachinas
    [SerializeField] private SlingShot_Handler _slingShotHandler;
    // Imagen (botón) de acceso al siguiente nivel
    [SerializeField] private Image _nextLevelImage;

    //Número de tiros gastados
    private int _usedNumberOfShots;

    // Gestor de iconos de UI
    private IconHandler _iconHandler;

    // Lista de cerdos en escena
    private List<Piggie> _piggies = new List<Piggie>();

    // Awake: detección del gestor de iconos, almacenamiento de cerdos en escena
    // y pestaña de cambio de nivel desactivada
    private void Awake()
    {
        // Patrón singleton
        if (instance == null)
        {
            instance = this;
        }

        _iconHandler = FindFirstObjectByType<IconHandler>();

        Piggie[] piggies = Object.FindObjectsByType<Piggie>(FindObjectsSortMode.None);
        for (int i = 0; i < piggies.Length; i++)
        {
            _piggies.Add(piggies[i]);
        }

        _nextLevelImage.enabled = false;
    }

    // UseShot: actualización del número de lanzamientos y de la UI.
    // Se comprueba si es el último tiro
    public void UseShot()
    {
        _usedNumberOfShots++;
        _iconHandler.UseShot(_usedNumberOfShots);
        CheckForLastShot();
    }

    // HasEnoughShots: comprobación de la existencia de tiros disponibles (Usada en Slingshot_Handler)
    public bool HasEnoughShots()
    {
        if (_usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // CheckForLastShot: Comprobación del último tiro
    public void CheckForLastShot()
    {
        if (_usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    // CheckAfterWaitTime: Tiempo de espera hasta reiniciar el nivel
    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(_secondsToWaitBeforeDeathCheck);

        if (_piggies.Count == 0)
        {
            WinGame();
        }
        else
        {
            RestartGame();
        }
    }

    // RemovePiggie: borra un cerdo y comprueba si queda alguno en pantalla
    public void RemovePiggie(Piggie piggie)
    {
        _piggies.Remove(piggie);
        CheckForAllDeadPiggies();
    }

    // RemovePiggie: comprueba si quedan cerdos en pantalla para activar el estado de victoria
    private void CheckForAllDeadPiggies()
    {
        if (_piggies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win/Lose

    // WinGame: se desactiva el tirachinas y aparece la interfaz de reinicio y siguiente nivel, si lo hay
    private void WinGame()
    {
        _restartScreenObject.SetActive(true);
        _slingShotHandler.enabled = false;

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int maxLevels = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex + 1 < maxLevels)
        {
            _nextLevelImage.enabled = true;
        }
    }

    // RestartGame: se reinicia el nivel
    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // RestartGame: se carga el siguiente nivel
    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #endregion

    // ExitGame: cierre del juego
    public void ExitGame () {
        Application.Quit ();
        Debug.Log("Bye!");
    }
}
