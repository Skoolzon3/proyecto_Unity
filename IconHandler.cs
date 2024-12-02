using UnityEngine;
using UnityEngine.UI;

public class IconHandler : MonoBehaviour
{
    // Listado de iconos de la UI
    [SerializeField] private Image[] _icons;

    // Color utilizado para indicar que un pájaro se ha lanzado
    [SerializeField] private Color _usedColor;

    // UseShot: Actualización de iconos de pájaros en la UI.
    // Se colorean los iconos en función de los pájaros utilizados
    public void UseShot(int shotNumber)
    {
        for (int i = 0; i < _icons.Length; i++)
        {
            if (shotNumber == i + 1)
            {
                _icons[i].color = _usedColor;
                return;
            }
        }
    }
}
