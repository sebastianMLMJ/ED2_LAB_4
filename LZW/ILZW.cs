using System;
using System.Collections.Generic;
using System.Text;

namespace LZW
{
    interface ILZW
    {
        void LZWAlgoritmo(string dirLectura, string dirEscritura, string dirEscritura2)
        {

        }

        void LZWAlgoritmo2(string dirLectura, string dirEscritura)
        {

        }

        void Comprimir(string dirLectura, string dirEscritura, string dirEscritura2)
        {

        }

        void descomprimir(string dirLectura, string dirEscritura)
        {

        }

        void Escritura(string text, string path)
        {

        }

        Dictionary<int, string> obtenerDiccionarioDescompresion()
        {
            Dictionary<int, string> diccionario = new Dictionary<int, string>();

            return diccionario;
        }

        Dictionary<string, int> obtenerDiccionarioCompresion()
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>();

            return diccionario;
        }

        void ByteArrayToFile(string fileName, byte[] byteArray) { 
        
        }



    }

}
