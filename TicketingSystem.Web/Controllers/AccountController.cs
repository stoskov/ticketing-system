﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TicketingSystem.Data;
using TicketingSystem.Models;
using TicketingSystem.Web.Models.Accounts;

namespace TicketingSystem.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		public AccountController() 
		{
			this.IdentityManager = new AuthenticationIdentityManager(new IdentityStore(new AppDbContext()));
		}

		public AccountController(AuthenticationIdentityManager manager)
		{
			this.IdentityManager = manager;
		}

		public AuthenticationIdentityManager IdentityManager { get; private set; }

		private Microsoft.Owin.Security.IAuthenticationManager AuthenticationManager
		{
			get
			{
				return this.HttpContext.GetOwinContext().Authentication;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			this.ViewBag.ReturnUrl = returnUrl;
			return this.View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (this.ModelState.IsValid)
			{
				// Validate the password
				IdentityResult result = await this.IdentityManager.Authentication.CheckPasswordAndSignInAsync(this.AuthenticationManager, model.UserName, model.Password, model.RememberMe);
				if (result.Success)
				{
					return this.RedirectToLocal(returnUrl);
				}
				else
				{
					this.AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return this.View(model);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return this.View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (this.ModelState.IsValid)
			{
				// Create a local login before signing in the user
				var user = new AppUser(model.UserName);
				var result = await this.IdentityManager.Users.CreateLocalUserAsync(user, model.Password);
				if (result.Success)
				{
					await this.IdentityManager.Authentication.SignInAsync(this.AuthenticationManager, user.Id, isPersistent: false);
					return this.RedirectToAction("Index", "Home");
				}
				else
				{
					this.AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return this.View(model);
		}

		//
		// POST: /Account/Disassociate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
		{
			string message = null;
			IdentityResult result = await this.IdentityManager.Logins.RemoveLoginAsync(this.User.Identity.GetUserId(), loginProvider, providerKey);
			if (result.Success)
			{
				message = "The external login was removed.";
			}
			else
			{
				message = result.Errors.FirstOrDefault();
			}

			return this.RedirectToAction("Manage", new { Message = message });
		}

		//
		// GET: /Account/Manage
		public async Task<ActionResult> Manage(string message)
		{
			this.ViewBag.StatusMessage = message ?? "";
			this.ViewBag.HasLocalPassword = await this.IdentityManager.Logins.HasLocalLoginAsync(this.User.Identity.GetUserId());
			this.ViewBag.ReturnUrl = this.Url.Action("Manage");
			return this.View();
		}

		//
		// POST: /Account/Manage
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			string userId = this.User.Identity.GetUserId();
			bool hasLocalLogin = await this.IdentityManager.Logins.HasLocalLoginAsync(userId);
			this.ViewBag.HasLocalPassword = hasLocalLogin;
			this.ViewBag.ReturnUrl = this.Url.Action("Manage");
			if (hasLocalLogin)
			{ 
				if (this.ModelState.IsValid)
				{
					IdentityResult result = await this.IdentityManager.Passwords.ChangePasswordAsync(this.User.Identity.GetUserName(), model.OldPassword, model.NewPassword);
					if (result.Success)
					{
						return this.RedirectToAction("Manage", new { Message = "Your password has been changed." });
					}
					else
					{
						this.AddErrors(result);
					}
				}
			}
			else
			{
				// User does not have a local password so remove any validation errors caused by a missing OldPassword field
				ModelState state = this.ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (this.ModelState.IsValid)
				{
					// Create the local login info and link it to the user
					IdentityResult result = await this.IdentityManager.Logins.AddLocalLoginAsync(userId, this.User.Identity.GetUserName(), model.NewPassword);
					if (result.Success)
					{
						return this.RedirectToAction("Manage", new { Message = "Your password has been set." });
					}
					else
					{
						this.AddErrors(result);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return this.View(model);
		}

		//
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, this.Url.Action("ExternalLoginCallback", "Account", new { loginProvider = provider, ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string loginProvider, string returnUrl)
		{
			ClaimsIdentity id = await this.IdentityManager.Authentication.GetExternalIdentityAsync(this.AuthenticationManager);
			// Sign in this external identity if its already linked
			IdentityResult result = await this.IdentityManager.Authentication.SignInExternalIdentityAsync(this.AuthenticationManager, id);
			if (result.Success) 
			{
				return this.RedirectToLocal(returnUrl);
			}
			else if (this.User.Identity.IsAuthenticated)
			{
				// Try to link if the user is already signed in
				result = await this.IdentityManager.Authentication.LinkExternalIdentityAsync(id, this.User.Identity.GetUserId());
				if (result.Success)
				{
					return this.RedirectToLocal(returnUrl);
				}
				else 
				{
					return this.View("ExternalLoginFailure");
				}
			}
			else
			{
				// Otherwise prompt to create a local user
				this.ViewBag.ReturnUrl = returnUrl;
				this.ViewBag.LoginProvider = loginProvider;
				return this.View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = id.Name });
			}
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (this.User.Identity.IsAuthenticated)
			{
				return this.RedirectToAction("Manage");
			}
            
			if (this.ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				IdentityResult result = await this.IdentityManager.Authentication.CreateAndSignInExternalUserAsync(this.AuthenticationManager, new AppUser(model.UserName));
				if (result.Success)
				{
					return this.RedirectToLocal(returnUrl);
				}
				else
				{
					this.AddErrors(result);
				}
			}

			this.ViewBag.ReturnUrl = returnUrl;
			return this.View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			this.AuthenticationManager.SignOut();
			return this.RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return this.View();
		}

		[AllowAnonymous]
		[ChildActionOnly]
		public ActionResult ExternalLoginsList(string returnUrl)
		{
			this.ViewBag.ReturnUrl = returnUrl;
			return (ActionResult)this.PartialView("_ExternalLoginsListPartial", new List<AuthenticationDescription>(this.AuthenticationManager.GetExternalAuthenticationTypes()));
		}

		[ChildActionOnly]
		public ActionResult RemoveAccountList()
		{
			return Task.Run(async () =>
			{
				var linkedAccounts = new List<IUserLogin>(await this.IdentityManager.Logins.GetLoginsAsync(this.User.Identity.GetUserId()));
				this.ViewBag.ShowRemoveButton = linkedAccounts.Count > 1;
				return (ActionResult)this.PartialView("_RemoveAccountPartial", linkedAccounts);
			}).Result;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.IdentityManager != null)
			{
				this.IdentityManager.Dispose();
				this.IdentityManager = null;
			}
			base.Dispose(disposing);
		}

		#region Helpers
		
		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				this.ModelState.AddModelError("", error);
			}
		}
		
		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (this.Url.IsLocalUrl(returnUrl))
			{
				return this.Redirect(returnUrl);
			}
			else
			{
				return this.RedirectToAction("Index", "Home");
			}
		}
		
		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUrl)
			{
				this.LoginProvider = provider;
				this.RedirectUrl = redirectUrl;
			}
			
			public string LoginProvider { get; set; }
			
			public string RedirectUrl { get; set; }
			
			public override void ExecuteResult(ControllerContext context)
			{
				context.HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties() { RedirectUrl = this.RedirectUrl }, this.LoginProvider);
			}
		}
		
		#endregion
	}
}