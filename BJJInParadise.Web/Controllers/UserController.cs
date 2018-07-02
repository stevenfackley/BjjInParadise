using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BjjInParadise.Business;
using BjjInParadise.Core.Constants;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.Models;
using BJJInParadise.Web.ViewModels;

namespace BJJInParadise.Web.Controllers
{
    [Authorize(Roles = UserConsts.ADMIN_ROLE)]
    public class UserController : BaseController
    {
        private AccountService _accountService;
        private ApplicationUserManager _userManager;
        private BookingService _bookingService ;

        public UserController(AccountService service, ApplicationUserManager userManager, BookingService bookingService)
        {
            _accountService = service;
            _bookingService = bookingService;
            _userManager = userManager;
        }
        // GET: User
        public async Task<ActionResult> Index()
        {
            var result = _accountService.GetAll().ToList();

            var mappedResult = Mapper.Map<IEnumerable<User>, List<UserViewModel>>(result);
            foreach (var userViewModel in mappedResult)
            {
                await AddUserRole(userViewModel);
            }
            return View(mappedResult);
        }

        private async Task AddUserRole(UserViewModel userViewModel)
        {
            try
            {
                userViewModel.IsAdmin = await _userManager.IsInRoleAsync(userViewModel.AspNetUserId, UserConsts.ADMIN_ROLE);
            }
            catch (Exception e)
            {
                userViewModel.IsAdmin = false;
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            var result = _accountService.Get(id);

            var mappedResult = Mapper.Map<User, UserBookingViewModel>(result);

            var user = _accountService.Get(id);

            await AddUserRole(mappedResult);

            var bookings = await _bookingService.GetBookingsByUserIdAsync(id);
            var booksEnum = new List<Booking>();
            foreach (var item in bookings)
            {
                item.User = user;
                booksEnum.Add(item);
            }

            mappedResult.BookedCamps = booksEnum;

            return View(mappedResult);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var user =  _accountService.Get(id);

            var mappedResult = Mapper.Map<User, UserBookingViewModel>(user);
            await AddUserRole(mappedResult); var bookings = await _bookingService.GetBookingsByUserIdAsync(id);
            
            mappedResult.BookedCamps = bookings;


            return View(mappedResult);
        }
        private async Task HandleRoleChange(bool isUserCurrentlyAdmin, User oldUser, UserBookingViewModel newUser)
        {
            if (isUserCurrentlyAdmin != newUser.IsAdmin)
            {
                if (newUser.IsAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(oldUser.AspNetUserId, UserConsts.USER_ROLE);
                    await _userManager.AddToRoleAsync(oldUser.AspNetUserId, UserConsts.ADMIN_ROLE);
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(oldUser.AspNetUserId, UserConsts.ADMIN_ROLE);
                    await _userManager.AddToRoleAsync(oldUser.AspNetUserId, UserConsts.USER_ROLE);
                }
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserBookingViewModel usrBookingViewModel)
        {
         

            if (ModelState.IsValid)
            {
                var user = _accountService.Get(usrBookingViewModel.UserId);
                var isUserCurrentlyAdmin = await _userManager.IsInRoleAsync(user.AspNetUserId, UserConsts.ADMIN_ROLE);
                await HandleRoleChange(isUserCurrentlyAdmin, user, usrBookingViewModel);
                var aspNetUser = await _userManager.FindByIdAsync(usrBookingViewModel.AspNetUserId);
                if (aspNetUser.PhoneNumber != usrBookingViewModel.PhoneNumber)
                {
                    aspNetUser.PhoneNumber = usrBookingViewModel.PhoneNumber;
                    await _userManager.UpdateAsync(aspNetUser);
                }
      
                var result2 = await _accountService.UpdateAsync(Mapper.Map<UserBookingViewModel, User>(usrBookingViewModel));
                return RedirectToAction("Index");
            }
            return View(usrBookingViewModel);
        }

        public ActionResult Delete(int id)
        {
            var user = _accountService.Get(id);
            return View(user);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(User user)
        {
            try
            {
                if (user.AspNetUserId != null)
                    await _userManager.DeleteAsync(await _userManager.FindByIdAsync(user.AspNetUserId));
            }
            catch (Exception e)
            {
             //ignored
            }
      
            await _accountService.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}