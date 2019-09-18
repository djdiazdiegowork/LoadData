using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.PMI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PMILoadDataClient;
using PMIWeb.Models;

namespace PMIWeb.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin, Buscador")]
    public class FinderController : Controller
    {
        private static Dictionary<string, byte[]> tempData = new Dictionary<string, byte[]>();
        private IConfiguration _iConfig;
        private IHttpContextAccessor _iHttpContextAccessor;
        private IHostingEnvironment _ihostingEnvironment;
        public FinderController(IConfiguration iConfig, IHttpContextAccessor iHttpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
           _iConfig = iConfig;
            _iHttpContextAccessor = iHttpContextAccessor;
            _ihostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Action para mostrar la pagina principal de la busqueda
        /// de resultados.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action que se va a utilizar para obtener los datos de
        /// un query dado por la vista principal.
        /// </summary>
        /// <param name="formModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(FormModel formModel)
        {
            // Obteniendo los atributos select
            var attributesSelect = Utils.Utils.GetAttributeSelects();
            var attributesFilters = Utils.Utils.GetAttributeFilters(formModel);

            var attributesToProcess = new List<AttributeBase>();
            attributesToProcess.AddRange(attributesSelect);
            attributesToProcess.AddRange(attributesFilters);
            var loadDataClient =
                new LoadDataClient(_iConfig.GetSection("service_url").Value, 1000, attributesToProcess, true);
            //await loadDataClient.Connect(_iHttpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString());
            //var data = await loadDataClient.FetchData(PageOperation.Next);
            var data = Utils.Utils.GetDataTest();
            return new JsonResult(new {status = "success", data});
        }


        /// <summary>
        /// Action que se va a utilizar para devolver por ajax
        /// los datos de una tabla a partir de un criterio de busqueda
        /// </summary>
        /// <param name="q"></param>
        /// <param name="tempTable"></param>
        /// <param name="treeView">parametro que indica el caso en que
        /// se quiera retornar un jason para un control treeview
        /// o para un control select2</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetDataFrom(string q, string tempTable, bool treeView = false)
        {
            IEnumerable<Select2Model> result = new List<Select2Model>();
            var loadDataClient = new LoadDataClient(_iConfig.GetSection("service_url").Value);
            var serviceResult = await loadDataClient.GetDataFrom(tempTable, q);

            if (serviceResult != null)
            {
                result = serviceResult.Select(r => new Select2Model {id = "1", text = r});
            }

            if (treeView)
            {
                //serviceResult = new List<string>() {"a\\b\\c", "a\\d", "b\\c\\d", "a\\b\\d", "a\\b\\e", "a\\b\\f", "a\\b\\g", "a\\b\\h", "a\\b\\i" };

                return Json(new {items = Utils.Utils.GetTreeViewModelFromPath(serviceResult).nodes});
            }
            return Json(new {items = result});
        }

        /// <summary>
        /// Action que se va a utilizar para crear un documento que va a ser descargado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestFormLimits(ValueCountLimit = 2000,Order = 1)]
        public IActionResult CreateDocument(IEnumerable<PROGRAMTB> data, string format)
        {
            ////Creando el documento temporal en el server
            var fileName = Utils.Utils.SaveDocumentTemp(format, data, tempData);
            return new JsonResult(new {FileGuid = fileName, FileName = $"Reporte-{DateTime.Now}.{format}"});
        }

        /// <summary>
        /// Action que se va a utilizar para descargar un documento que se encuentra en la
        /// variable temporal del sistema
        /// </summary>
        /// <param name="fileGuid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IActionResult DownloadDocument(string fileGuid, string fileName)
        {
            if (tempData.ContainsKey(fileGuid))
            {
                byte[] data = tempData[fileGuid];
                tempData.Remove(fileGuid);
                return File(data, "application/vnd.ms-excel", fileName);
            }
            return new EmptyResult();
        }
    }
}