using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;

namespace BJJInParadise.Web.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private AccountService _accountService;


        public AdminController(AccountService service)
        {
            _accountService = service;
        }
        // GET: Admin
        
        public ActionResult Index()
        {
            var users = _accountService.GetAll();
            return View(users);
        }

     

        // GET: Admin/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

     

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

       
    }
}
