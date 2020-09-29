using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace AplicacaoDesafio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        public static IWebHostEnvironment _enviroment;

        public FileUploadController(IWebHostEnvironment enviroment)
        {
            _enviroment = enviroment;
        }

       
        public class FileUploadAPI { 
            public IFormFile files { get; set; }
        }

        [HttpPost]
        public async Task<JsonArray> Post([FromForm]FileUploadAPI objFile) {

            string path = _enviroment.WebRootPath;
            JsonArray jsonArquivos = new JsonArray();

            try
            {
                if (objFile.files.Length > 0)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream fileStream = System.IO.File.Create(path + objFile.files.FileName))
                    {
                        objFile.files.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                        jsonArquivos = verificarArquivos(path + objFile.files.FileName);
                        return jsonArquivos;
                    }
                }
                else
                {
                    jsonArquivos = null;
                    return jsonArquivos;
                }
            }
            catch {
                jsonArquivos = null;
                return jsonArquivos;
            }
        }

        public JsonArray verificarArquivos(string filePath)
        {
            dynamic jsonTotal = new JsonArray();
                using (ZipArchive arquivoZip = ZipFile.OpenRead(filePath))
                {
                    foreach (ZipArchiveEntry entry in arquivoZip.Entries)
                    {
                        dynamic json = new JsonObject();
                        json.nome = entry.Name;
                        json.extensao = entry.Name.Substring(entry.Name.Length - 3);
                        json.tamanho = entry.Length + " Bytes";

                        jsonTotal.Add(json);
                    }
                }

            return jsonTotal;
        }
    }
}
