﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mango.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
        {
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Login()
		{
			LoginResponseDto loginResponseDto = new();
			return View(loginResponseDto);
		}
		
		[HttpGet]
		public IActionResult Register()
		{
			var roleList = new List<SelectListItem>()
			{
				new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
				new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
			};

			ViewBag.RoleList = roleList;

			return View();
		}
		
		[HttpPost]
		public async Task<IActionResult> Register(RegisterationRequestDto obj)
		{
			ResponseDto result = await _authService.RegisterAsync(obj);
			ResponseDto assignRole;

			if (result != null && result.IsSuccess)
			{
				if (string.IsNullOrEmpty(obj.Role))
				{
					obj.Role = SD.RoleCustomer;
				}
				assignRole = await _authService.AssignRoleAsync(obj);
				if (assignRole != null && assignRole.IsSuccess)
				{
					TempData["success"] = "Registration Successfull";
					return RedirectToAction(nameof(Login));
				}
			}

			var roleList = new List<SelectListItem>()
			{
				new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
				new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
			};

			ViewBag.RoleList = roleList;

			return View(result);
		}
		
		public IActionResult Logout()
		{
			return View();
		}
	}
}
