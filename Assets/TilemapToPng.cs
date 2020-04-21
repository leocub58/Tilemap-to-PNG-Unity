using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TilemapToPng))]
public class TilemapToPngEditor : Editor
{
    string nombre = "";

    public override void OnInspectorGUI ()
    {
        TilemapToPng GTM = (TilemapToPng)target;


        //DrawDefaultInspector();

        if (GTM.ImagenLista == null)
        {
            if (GUILayout.Button("Empacar como PNG"))
            {
                GTM.Empacar();
            }
        }
        else
        {
            GUILayout.Label("Nombre del archivo");
            nombre = GUILayout.TextField(nombre);
            if(nombre.Length > 0)
            {
                if (GUILayout.Button("Exportar archivo"))
                {
                    GTM.ExportarPng(nombre);
                }
            }
            
        }
            
        
    }

}
#endif



    public class TilemapToPng : MonoBehaviour
{

    Tilemap tm;

    int minX = 0;
    int maxX = 0;
    int minY = 0;
    int maxY = 0;
    
    public Texture2D ImagenLista;

    public void Empacar()
    {
        tm = GetComponent<Tilemap>();
        Sprite SpriteCualquiera = null;


        for (int x = 0; x < tm.size.x; x++) //Hallamos el punto menor y mayor
        {
            for (int y = 0; y < tm.size.y; y++)
            {
                Vector3Int pos = new Vector3Int(-x, -y, 0);
                if (tm.GetSprite(pos) == null)
                {
                    print("no hay sprite en esta pos");
                }
                else
                {
                    SpriteCualquiera = tm.GetSprite(pos); //seleccionamos un sprite cualquiera para más tarde saber las dimensiones de los sprites
                    if (minX > pos.x)
                    {
                        minX = pos.x;
                    }
                    if (minY > pos.y)
                    {
                        minY = pos.y;
                    }
                }

                pos = new Vector3Int(x, y, 0);
                if (tm.GetSprite(pos) == null)
                {
                    print("no hay sprite en esta pos");
                }
                else
                {
                    if (maxX < pos.x)
                    {
                        maxX = pos.x;
                    }
                    if (maxY < pos.y)
                    {
                        maxY = pos.y;
                    }
                }
            }
        }


        //Hallamos el tamaño del sprite en pixeles
        float width = SpriteCualquiera.rect.width;
        float height = SpriteCualquiera.rect.height;


        //creamos una textura con el tamaño multiplicado por el numero de celdas
        Texture2D Imagen = new Texture2D((int)width * tm.size.x, (int)height * tm.size.y);

        //Asignamos toda la imagen invisible
        Color[] invisible = new Color[Imagen.width * Imagen.height];
        for (int i = 0; i < invisible.Length; i++)
        {
            invisible[i] = new Color(0f, 0f, 0f, 0f);
        }
        Imagen.SetPixels(0,0,Imagen.width, Imagen.height, invisible);
        

        //Ahora asignamos a cada bloque sus respectivos pixeles
        for (int x = minX; x <= maxX; x++)
        {
            for(int y = minY; y <= maxY; y++)
            {
                if (tm.GetSprite(new Vector3Int(x, y, 0)) == null)
                {
                    print("Bloque vacio");
                }
                else
                {
                    //mapeamos los pixeles para que el minX = 0 y minY = 0
                    Imagen.SetPixels((x - minX) * (int)width, (y - minY) * (int)height, (int)width, (int)height, GetCurrentSprite(tm.GetSprite(new Vector3Int(x, y, 0))).GetPixels()   );
                }
            }
        }
        Imagen.Apply();

        ImagenLista = Imagen; //Almacenamos la textura de la imagen lista
    }

    Texture2D GetCurrentSprite(Sprite sprite) //metodo para obtener el sprite recortado tal y como lo ponemos
    {
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                         (int)sprite.textureRect.y,
                                         (int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);

        Texture2D textura = new Texture2D((int)sprite.textureRect.width,
                                         (int)sprite.textureRect.height);

        textura.SetPixels(pixels);
        textura.Apply();
        return textura;
    }

     public void ExportarPng (string nombre) //metodo que exporta como png
     {
         byte[] bytes = ImagenLista.EncodeToPNG();
         var dirPath = Application.dataPath + "/Exported Tilemaps/";
         if (!Directory.Exists(dirPath))
         {
             Directory.CreateDirectory(dirPath);
         }
         File.WriteAllBytes(dirPath + nombre + ".png", bytes);
        ImagenLista = null;
     }

}
