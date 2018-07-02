using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.ViewModels;

namespace BJJInParadise.Web.Controllers
{
    public class CampController : Controller
    {
        private readonly CampService _campService;

        public CampController(CampService campService)
        {
          
            _campService = campService;
        }
        // GET: Camp
        [Authorize]
        public ActionResult Index()
        {
            var camps = _campService.GetAll();
            return View(camps);
        }

        // POST: Admin/Create
        [HttpGet]
        public ActionResult Create()
        {

            return View(new CreateCampViewModel());
        }

        // POST: Admin/Create
        [HttpPost]
        public async Task<ActionResult> Create(CreateCampViewModel camp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var create = Mapper.Map<CreateCampViewModel, Camp>(camp);
                    // TODO: Add insert logic here
                    await _campService.AddAsync(create);
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(camp);
                }
            }
            else
            {
                return View(camp);
            }
        }

        // POST: Admin/Edit/5
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var camp = _campService.Get(id);
            return View(camp);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit( Camp camp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add update logic here
                    await _campService.UpdateAsync(camp);
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                return View(camp);
            }
        }

        public ActionResult ViewCamp(int id)
        {
            var camp = _campService.Get(id);
            return View(camp);
        }
        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_campService.Get(id));
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(Camp camp)
        {
            try
            {
                await _campService.DeleteAsync(camp);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                
                return View();
            }
        }

        public ActionResult Details(int id)
        {
            return View(_campService.Get(id));
        }

        public ActionResult Upcoming()
        {
            var camps = _campService.GetAll();
            return View(camps);
        }
    }
}