using System;
using System.Collections.Generic;
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
    public class CampRoomOptionController : BaseController
    {
        private readonly CampRoomOptionService _campRoomOptionService;

        public CampRoomOptionController(CampRoomOptionService campService)
        {

            _campRoomOptionService = campService;
        }
        // GET: CampRoomOption
        public async Task<ActionResult> Index(int id)
        {
            var result = await _campRoomOptionService.GetActiveOptionsByCampIdAsync(id);
            
            var model = new CreateCampRoomOptionViewModel { CampId = id , CampRoomOptions = result};
            return PartialView("_Index", model);
        }

        public ActionResult Create(int id)
        {
            var model = new CampRoomOption {CampId = id};
            return View("Create", model);
        }

        // POST: Admin/Create
        [HttpPost]
        public async Task<ActionResult> Create(CampRoomOption camp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _campRoomOptionService.AddAsync(camp);
                    return RedirectToAction("Details", "Camp", new {id = camp.CampId});
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

        public ActionResult Edit(int id)
        {
            var result = _campRoomOptionService.Get(id);

            return View(result);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(CampRoomOption camp)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _campRoomOptionService.UpdateAsync(camp);
                    return RedirectToAction("Details","Camp", new {id =camp.CampId});
                }
                catch
                {
                    return RedirectToAction("Edit", new {id = camp.CampId});
                }
            }
            else
            {
                return View(camp);
            }
        }


        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View(_campRoomOptionService.Get(id));
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(CampRoomOption camp)
        {
            try
            {
                await _campRoomOptionService.DeleteAsync(camp);
                return RedirectToAction("Index", "Camp");
            }
            catch (Exception e)
            {

                return View(camp);
            }
        }


    }
}