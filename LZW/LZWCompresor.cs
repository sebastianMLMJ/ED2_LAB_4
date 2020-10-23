using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace LZW
{
    public class LZWCompresor:ILZW
    {
        public void LZWAlgoritmo(string dirLectura, string dirEscritura, string dirEscritura2)
        {
            Comprimir(dirLectura, dirEscritura, dirEscritura2);
        }
        public void LZWAlgoritmo2(string dirLectura, string dirEscritura)
        {
            descomprimir(dirLectura, dirEscritura);
        }
        #region Comprimir
        public void Comprimir(string dirLectura, string dirEscritura, string dirEscritura2)
        {
            #region Variables

            int LongitudMaxDiccionario = 256;
            int LongitudByte = 8;

            #endregion

            #region Caracteres

            var diccionario = obtenerDiccionarioCompresion();
            LongitudMaxDiccionario = diccionario.Count;

            #endregion
            //Analizar texto, creando diccionario y escribiendo en archivo
            #region Algoritmo

            int LongitudBuffer = 1024;

            //Empezar concatenar
            string c = string.Empty;
            List<int> comprimir = new List<int>();

            string bits = "";
            int contador = 0;

            //Buffer para comprimir
            using (var file = new FileStream(dirLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: LongitudBuffer);

                        foreach (var t in buffer)
                        {
                            string ct = c + ((char)t);
                            if (diccionario.ContainsKey(ct))
                            {
                                c = ct;
                            }
                            else
                            {
                                //sacarlo
                                string ByteString = Convert.ToString(diccionario[c], 2).PadLeft(LongitudByte, '0'); // produce cadena "00111111";
                                bits += ByteString;

                                while (bits.Length >= 8) //Escribir bytes en archivo 
                                {
                                    string Byte = bits.Substring(0, 8);
                                    bits = bits.Remove(0, 8);

                                    ByteArrayToFile(dirEscritura, new[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                                }

                                comprimir.Add(diccionario[c]);
                                //Aqui ya lo concatena y lo agrega
                                diccionario.Add(ct, diccionario.Count);
                                c = ((char)t).ToString();
                                //Verificar tamaño de los bits
                                if (diccionario.Count >= LongitudMaxDiccionario)
                                {
                                    LongitudByte++;
                                    LongitudMaxDiccionario = (int)Math.Pow(2, LongitudByte);
                                }
                            }
                        }
                    }

                }

            }
            //Ultima cadena del archivo
            if (!string.IsNullOrEmpty(c))
            {
                string ByteString = Convert.ToString(diccionario[c], 2).PadLeft(LongitudByte, '0'); // produce cadena "00111111";
                bits += ByteString;

                comprimir.Add(diccionario[c]);
            }

            if (bits != "") //Bits restantes
            {
                while (bits.Length % 8 != 0)
                {
                    bits += "0";
                }
                while (bits.Length >= 8) //Escribir bytes en archivo 
                {
                    string Byte = bits.Substring(0, 8);
                    bits = bits.Remove(0, 8);

                    ByteArrayToFile(dirEscritura, new[] { Convert.ToByte(Convert.ToInt32(Byte, 2)) });
                }
            }

            #endregion

            #region FileInfo

            FileInfo Fileoriginal= new FileInfo(dirLectura);
            FileInfo FileCompreso = new FileInfo(dirEscritura);
            InfCompresion.agregarNuevaCompresion(new InfCompresion(Path.GetFileName(dirLectura), Fileoriginal.Length, FileCompreso.Length, dirEscritura2)); //Anadir a mis compresiones

            #endregion
        }
        #endregion
        public void descomprimir(string dirLectura, string dirEscritura)
        {


            int LongitudBuffer = 1024;

            int LongitudByte = 8;
            int LongitudMaxDiccionario = 256;

            string bits = "";

            Dictionary<int, char> diccionario  = new Dictionary<int, char>();

            using (var file = new FileStream(dirLectura, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {
                    //Definir diccionario

                    Dictionary<int, string> diccionario2 = obtenerDiccionarioDescompresion();

                    //Operacion inicial

                    int key = 0;
                    string c = diccionario2[(int)reader.ReadByte()];
                    string descomprimir = c;

                    //Buffer para descomprimir
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        var buffer = reader.ReadBytes(count: LongitudBuffer);

                        foreach (var t in buffer)
                        {

                            if (diccionario2.Count + 1 >= LongitudMaxDiccionario)
                            {
                                LongitudByte++;
                                LongitudMaxDiccionario = (int)Math.Pow(2, LongitudByte);
                            }

                            string ByteString = Convert.ToString(t, 2).PadLeft(8, '0'); // produce cadena "00111111";
                            bits += ByteString;

                            if (bits.Length >= LongitudByte)
                            {
                                key = Convert.ToInt32(bits.Substring(0, LongitudByte), 2);
                                bits = bits.Remove(0, LongitudByte);

                                string entry = null;
                                if (diccionario2.ContainsKey(key))
                                {
                                    entry = diccionario2[key];
                                }
                                else if (key == diccionario2.Count)
                                {
                                    entry = c + c[0];
                                }

                                descomprimir += entry;

                                //  Agregar nueva frase al diccionario

                                if (entry != null)
                                {
                                    diccionario2.Add(diccionario2.Count, c + entry[0]);
                                }

                                c = entry;

                            }
                        }

                        Escritura(descomprimir, dirEscritura);
                        descomprimir = "";
                    }
                }
            }
        }

        public static void Escritura(string text, string path)
        {
            var buffer = new byte[text.Length];

            using (var file = new FileStream(path, FileMode.Append))
            {
                using (var writer = new BinaryWriter(file))
                {
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        buffer[i] = Convert.ToByte(text[i]);
                    }

                    writer.Write(buffer);
                }
            }
        }

        private static Dictionary<int, string> obtenerDiccionarioDescompresion()
        {
            Dictionary<int, string> diccionario = new Dictionary<int, string>();
            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(i, ((char)i).ToString());
            }

            return diccionario;
        }

        private static Dictionary<string, int> obtenerDiccionarioCompresion()
        {
            Dictionary<string, int> diccionario = new Dictionary<string, int>();
            for (int i = 0; i < 256; i++)
            {
                diccionario.Add(((char)i).ToString(), i);
            }

            return diccionario;
        }

        private static void ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception caught in process: {0}", ex);
            }
        }
    }
}
