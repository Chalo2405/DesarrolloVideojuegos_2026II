using System.IO;
using UnityEditor;
using UnityEngine;

// Script de editor: agrega el menú "Semana06 > Generar Tileset".
// Genera una imagen de 128 x 64 píxeles con 8 tiles de 32 x 32 píxeles.
public static class GeneradorTilesetSemana06
{
    const int T = 32; // tamaño de cada tile en píxeles

    [MenuItem("Semana06/Generar Tileset")]
    public static void Generar()
    {
        Texture2D tex = new Texture2D(T * 4, T * 2, TextureFormat.RGBA32, false);

        Color tierra = new Color(0.55f, 0.36f, 0.20f);
        Color tierraOscura = new Color(0.42f, 0.27f, 0.15f);
        Color pasto = new Color(0.30f, 0.70f, 0.25f);
        Color cielo = new Color(0.60f, 0.82f, 0.98f);
        Color gris = new Color(0.62f, 0.62f, 0.62f);
        Color grisOscuro = new Color(0.45f, 0.45f, 0.45f);
        Color transparente = new Color(0f, 0f, 0f, 0f);

        // Fila superior (índices 0 a 3 al cortar con el Sprite Editor)
        // 0: suelo con pasto arriba
        Rellenar(tex, 0, 1, 0, 0, T, T, tierra);
        Rellenar(tex, 0, 1, 0, 24, T, T, pasto);
        // 1: borde izquierdo (pasto arriba y a la izquierda)
        Rellenar(tex, 1, 1, 0, 0, T, T, tierra);
        Rellenar(tex, 1, 1, 0, 24, T, T, pasto);
        Rellenar(tex, 1, 1, 0, 0, 6, T, pasto);
        // 2: borde derecho (pasto arriba y a la derecha)
        Rellenar(tex, 2, 1, 0, 0, T, T, tierra);
        Rellenar(tex, 2, 1, 0, 24, T, T, pasto);
        Rellenar(tex, 2, 1, 26, 0, T, T, pasto);
        // 3: tierra con detalles
        Rellenar(tex, 3, 1, 0, 0, T, T, tierra);
        Rellenar(tex, 3, 1, 6, 6, 10, 10, tierraOscura);
        Rellenar(tex, 3, 1, 20, 16, 25, 21, tierraOscura);

        // Fila inferior (índices 4 a 7)
        // 4: fondo de cielo
        Rellenar(tex, 0, 0, 0, 0, T, T, cielo);
        Rellenar(tex, 0, 0, 8, 20, 12, 22, Color.white);
        Rellenar(tex, 0, 0, 22, 8, 26, 10, Color.white);
        // 5: fondo de cueva (gris, se tiñe con el color del DNI)
        Rellenar(tex, 1, 0, 0, 0, T, T, gris);
        Rellenar(tex, 1, 0, 0, 15, T, 17, grisOscuro);
        Rellenar(tex, 1, 0, 15, 0, 17, 15, grisOscuro);
        Rellenar(tex, 1, 0, 6, 17, 8, T, grisOscuro);
        // 6: decoración (flor sobre fondo transparente)
        Rellenar(tex, 2, 0, 0, 0, T, T, transparente);
        Rellenar(tex, 2, 0, 15, 0, 17, 14, pasto);
        Rellenar(tex, 2, 0, 11, 14, 21, 22, new Color(0.95f, 0.30f, 0.35f));
        Rellenar(tex, 2, 0, 14, 16, 18, 20, new Color(1f, 0.85f, 0.20f));
        // 7: primer plano (arbusto semitransparente)
        Rellenar(tex, 3, 0, 0, 0, T, T, transparente);
        Rellenar(tex, 3, 0, 2, 0, 30, 20, new Color(0.10f, 0.40f, 0.15f, 0.85f));
        Rellenar(tex, 3, 0, 6, 20, 26, 28, new Color(0.10f, 0.40f, 0.15f, 0.85f));

        tex.Apply();

        string carpeta = "Assets/Sprites/Semana06";
        Directory.CreateDirectory(carpeta);
        string ruta = carpeta + "/TilesetSemana06.png";
        File.WriteAllBytes(ruta, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(ruta);
        Debug.Log("Tileset generado en " + ruta + " (" + tex.width + " x " + tex.height + " píxeles)");
    }

    // Pinta un rectángulo dentro del tile ubicado en (columna, fila).
    // x0, y0 inclusive; x1, y1 exclusive; coordenadas locales del tile.
    static void Rellenar(Texture2D tex, int columna, int fila,
                         int x0, int y0, int x1, int y1, Color color)
    {
        for (int x = x0; x < x1; x++)
        {
            for (int y = y0; y < y1; y++)
            {
                tex.SetPixel(columna * T + x, fila * T + y, color);
            }
        }
    }
}