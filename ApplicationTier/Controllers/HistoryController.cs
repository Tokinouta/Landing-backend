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
        public ActionResult Create([FromBody] string[] collection)
        {
            Console.WriteLine(collection.ToString());
            var dateTimes = collection.Select(s =>
            {
                if (DateTime.TryParse(s, out DateTime dt))
                {
                    return dt;
                }
                else
                {
                    // 这种时间应该是不会有对应的记录的
                    return DateTime.MinValue;
                }
            });
            var t = _dbContext.BasicInformations
                .Where(s => dateTimes.Any(t => s.DateTime == t));
            List<IEnumerable<double>> data = new(t.Count());
            foreach (var item in t)
            {
                data.Add(MatlabReader.Read<double>(item.PathToData, "Vk_record").Column(0)
                        .ToList().Where((c, i) => i % 50 == 0));
            }
            //t.Select(s => );
            //var CalCount = MatlabReader.Read<double>(t.First().PathToData, "Vk_record").Column(0);
            var dates = _dbContext.BasicInformations.Take(3).Select(s => s.DateTime);

            return Ok(new { collection, data, dates });

            //return Ok(collection);
        }

        // GET: HistoryController/Edit/5
        [HttpGet]
        public ActionResult Edit()
        {
            var dates = _dbContext.BasicInformations.Take(3).Select(s => s.DateTime);

            return Ok(dates);
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
