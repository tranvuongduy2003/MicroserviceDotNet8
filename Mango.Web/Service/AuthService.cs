using Mango.Web.Models;
using Mango.Web.Service.IService;
using static Mango.Web.Utility.SD;

namespace Mango.Web.Service
{
	public class AuthService : IAuthService
	{
		private readonly IBaseService _baseService;
		public AuthService(IBaseService baseService)
        {
			_baseService = baseService;
		}

        public async Task<ResponseDto?> AssignRoleAsync(RegisterationRequestDto registerationRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = ApiType.POST,
				Data = registerationRequestDto,
				Url = CouponAPIBase + "/api/Auth/AssignRole"
			});
		}

		public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = ApiType.POST,
				Data = loginRequestDto,
				Url = CouponAPIBase + "/api/Auth/Login"
			});
		}

		public async Task<ResponseDto?> RegisterAsync(RegisterationRequestDto registerationRequestDto)
		{
			return await _baseService.SendAsync(new RequestDto()
			{
				ApiType = ApiType.POST,
				Data = registerationRequestDto,
				Url = CouponAPIBase + "/api/Auth/Register"
			});
		}
	}
}
