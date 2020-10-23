using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LZW;
using System.IO;

namespace API_LZW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LZWController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public LZWController(IWebHostEnvironment env)
        {
            _environment = env;
        }



        //-------- Metodos LZW --------------------
        LZWCompresor ComprimirLZW = new LZWCompresor();

        public void DescomprimirArchivos(IFormFile objFile)
        {
            string[] FileName1 = objFile.FileName.Split(".");
            ComprimirLZW.descomprimir(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, _environment.ContentRootPath + "\\ArchivosCompresos\\" + FileName1[0] + ".txt");

        }

        public void ComprimirArchivos(IFormFile objFile, string id)
        {
            string[] FileName1 = objFile.FileName.Split(".");

            ComprimirLZW.Comprimir(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, _environment.ContentRootPath + "\\ArchivosCompresos\\" + id + ".lzw", _environment.ContentRootPath + "\\ArchivosCompresos\\" + "Compresiones.txt");
            //ComprimirLZW.Comprimir(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, _environment.ContentRootPath + "\\ArchivosCompresos\\" + id + ".lzw", FileName1[0], _environment.ContentRootPath + "\\ArchivosCompresos\\" + "Compresiones.txt");

        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------

        [Route("/api/compressions")]
        [HttpGet]
        public IEnumerable<InfCompresion> DescargarCompresiones()
        {

            var memory = new MemoryStream();
            InfCompresion Datos = new InfCompresion();
            List<InfCompresion> ListaDatosCompres = new List<InfCompresion>();
            string[] split;
            try
            {
                using (var stream = new StreamReader(_environment.ContentRootPath + "\\ArchivosCompresos\\" + "Compresiones.txt"))
                {

                    while (!stream.EndOfStream)
                    {

                        split = stream.ReadLine().Split(",");
                        Datos.nombreOriginal = split[0];
                        Datos.razonDeCompresion = Convert.ToDouble(split[1]);
                        Datos.factorDeCompresion = Convert.ToDouble(split[2]);
                        Datos.porcentajeDeCompresion = Convert.ToDouble(split[3]);
                        Datos.RutaO = (split[4]);

                        ListaDatosCompres.Add(Datos);



                    }
                }

                return ListaDatosCompres;

            }
            catch (Exception)
            {

                return ListaDatosCompres;
            }




        }


        [Route("/api/compress/{id}")]
        [HttpPost]
        public async Task<IActionResult> SubirFileTxt([FromForm] IFormFile objFile, string id)
        {
            try
            {
                if (objFile.Length > 0)
                {
                    if (!Directory.Exists(_environment.ContentRootPath + "\\ArchivosCompresos\\")) Directory.CreateDirectory(_environment.ContentRootPath + "\\ArchivosCompresos\\");
                    using var _fileStream = System.IO.File.Create(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName);
                    objFile.CopyTo(_fileStream);
                    _fileStream.Flush();
                    _fileStream.Close();

                    ComprimirArchivos(objFile, id);
                    var memory = new MemoryStream();

                    using (var stream = new FileStream(_environment.ContentRootPath + "\\ArchivosCompresos\\" + id + ".lzw", FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, id + ".lzw");
                }

                return StatusCode(404, "Archivo Vacio");

            }
            catch
            {

                return StatusCode(404, "Error");
            }
        }

        //
        [Route("/api/decompress")]
        [HttpPost]
        public async Task<IActionResult> SubirFileHuff([FromForm] IFormFile objFile)
        {
            try
            {
                if (objFile.Length > 0)
                {
                    if (!Directory.Exists(_environment.ContentRootPath + "\\ArchivosCompresos\\")) Directory.CreateDirectory(_environment.ContentRootPath + "\\ArchivosCompresos\\");
                    using var _fileStream = System.IO.File.Create(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName);
                    objFile.CopyTo(_fileStream);
                    _fileStream.Flush();
                    _fileStream.Close();
                    DescomprimirArchivos(objFile);

                    var memory = new MemoryStream();
                    var name = objFile.FileName;
                    using (var stream = new FileStream(_environment.ContentRootPath + "\\ArchivosCompresos\\" + objFile.FileName, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }

                    memory.Position = 0;
                    return File(memory, System.Net.Mime.MediaTypeNames.Application.Octet, objFile.FileName);
                }
                else
                {
                    return StatusCode(404, "Archivo Vacio");
                }
            }
            catch
            {
                return StatusCode(404, "Error");
            }
        }
    }
}