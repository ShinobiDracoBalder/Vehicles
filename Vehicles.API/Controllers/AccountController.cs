using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vehicles.API.Data;
using Vehicles.API.Data.Entities;
using Vehicles.API.Helpers;
using Vehicles.API.Models;
using Vehicles.Common.Enums;
using Vehicles.Common.Models;

namespace Vehicles.API.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly DataContext _dataContext;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IImageHelper _imageHelper;

        public AccountController(IUserHelper userHelper,DataContext context
            , ICombosHelper combosHelper, IBlobHelper blobHelper, IImageHelper imageHelper)
        {
            _userHelper = userHelper;
            _dataContext = context;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
            _imageHelper = imageHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }
        public async Task<IActionResult> ChangeUser(){
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            EditUserViewModel model = new(){
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ImageId = user.ImageId,
                PicturePath = user.PicturePath,
                Id = user.Id,
                Document = user.Document,
                DocumentTypeId = user.DocumentType.Id,
                DocumentTypes = _combosHelper.GetComboDocumentTypes(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model){
            if (ModelState.IsValid){
                Guid imageId = model.ImageId;
                string Picture = model.PicturePath;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    model.PicturePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName = model.LastName ?? user.LastName;
                user.Address = model.Address ?? user.Address;
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
                user.ImageId = imageId;
                user.PicturePath = Picture;
                user.DocumentType = await _dataContext.DocumentTypes.FindAsync(model.DocumentTypeId);
                user.Document = model.Document ?? user.Document;
                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    IdentityResult result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(ChangeUser));
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                }
            }

            return View(model);
        }
        public IActionResult RecoverPassword()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                DocumentTypes = _combosHelper.GetComboDocumentTypes()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty;

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    model.PicturePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }

                User user = await _userHelper.AddUserAsync(model, imageId, UserType.User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado por otro usuario.");
                    model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
                    return View(model);
                }

                LoginViewModel loginViewModel = new LoginViewModel {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username, 
                };

                var result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded){
                    return RedirectToAction("Index", "Home");
                }

                //string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                //string tokenLink = Url.Action("ConfirmEmail", "Account", new
                //{
                //    userid = user.Id,
                //    token = myToken
                //}, protocol: HttpContext.Request.Scheme);

                //Response response = _mailHelper.SendMail(model.Username, "Vehicles - Confirmación de cuenta", $"<h1>Vehicles - Confirmación de cuenta</h1>" +
                //    $"Para habilitar el usuario, " +
                //    $"por favor hacer clic en el siguiente enlace: </br></br><a href = \"{tokenLink}\">Confirmar Email</a>");
                //if (response.IsSuccess)
                //{
                //    ViewBag.Message = "Las instrucciones para habilitar su cuenta han sido enviadas al correo.";
                //    return View(model);
                //}

               // ModelState.AddModelError(string.Empty, response.Message);
            }

            model.DocumentTypes = _combosHelper.GetComboDocumentTypes();
            return View(model);
        }
    }
}
