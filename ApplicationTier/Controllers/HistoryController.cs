using HistoryDemo;
using MathNet.Numerics.Data.Matlab;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationTier.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HistoryController : ControllerBase
    {
        readonly IConfiguration _configuration;
        readonly IServiceProvider _serviceProvider;
        readonly AppDbContext _dbContext;

        public HistoryController(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            AppDbContext context)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _dbContext = context;
        }

        // GET: HistoryController
        [HttpGet]
        public ActionResult Index()
        {
            var t = _dbContext.BasicInformations.First();
            var CalCount = MatlabReader.Read<double>(t.PathToData, "Vk_record").Column(0);
            return Ok(CalCount.ToList().Where((c, i) => i % 50 == 0));
        }

        // GET: HistoryController/Details/5
        [HttpGet]
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: HistoryController/Create
        [HttpGet]
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: HistoryController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(object collection)
        {
            Console.WriteLine(collection.ToString());
            var t = _dbContext.BasicInformations.First();
            var CalCount = MatlabReader.Read<double>(t.PathToData, "Vk_record").Column(0);
            return Ok(new { collection, data = CalCount.ToList().Where((c, i) => i % 50 == 0) });

            //return Ok(collection);
        }

        // GET: HistoryController/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: HistoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: HistoryController/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: HistoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }
    }
}
