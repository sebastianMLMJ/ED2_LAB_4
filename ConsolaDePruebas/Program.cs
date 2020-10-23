using System;
using LZW;


namespace ConsolaDePruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            LZWCompresor compresor = new LZWCompresor();
            compresor.Comprimir("C:\\Users\\Danilo_Toshiba\\Desktop\\ED2 LAB 4\\cuento.txt", "C:\\Users\\Danilo_Toshiba\\Desktop\\ED2 LAB 4\\compresion.txt", "C:\\Users\\Danilo_Toshiba\\Desktop\\ED2 LAB 4\\info.txt");
            compresor.descomprimir("C:\\Users\\Danilo_Toshiba\\Desktop\\ED2 LAB 4\\compresion.txt", "C:\\Users\\Danilo_Toshiba\\Desktop\\ED2 LAB 4\\descompresion.txt");

        }
    }
}
