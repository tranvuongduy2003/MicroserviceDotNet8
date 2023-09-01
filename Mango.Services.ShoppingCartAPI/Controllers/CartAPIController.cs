using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
	[Route("api/Cart")]
	[ApiController]
	public class CartAPIController : ControllerBase
	{
		private readonly AppDbContext _db;
		private readonly IMapper _mapper;
		private ResponseDto _responseDto;

		public CartAPIController(AppDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
			_responseDto = new ResponseDto();
		}

		[HttpGet("GetCart/{userId}")]
		public async Task<ResponseDto> GetCart(string userId)
		{
			try
			{
				CartDto cart = new()
				{
					CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId)),
				};
				cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

				foreach(var item in cart.CartDetails)
				{
					cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
				}

				_responseDto.Result = cart;
			}
			catch (Exception ex)
			{
				_responseDto.IsSuccess = false;
				_responseDto.Message = ex.Message;
			}
			return _responseDto;
		}

		[HttpPost("CartUpsert")]
		public async Task<ResponseDto> CartUpsert(CartDto cartDto)
		{
			try
			{
				var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
				if (cartHeaderFromDb == null)
				{
					// create cart header and details
					CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
					_db.CartHeaders.Add(cartHeader);
					await _db.SaveChangesAsync();
					cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
					_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails));
					await _db.SaveChangesAsync();
				}
				else
				{
					// if header is not null
					// check if details has same product
					var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
					if (cartDetailsFromDb == null)
					{
						// create cart details
						cartDto.CartDetails.First().CartHeaderId = cartDto.CartHeader.CartHeaderId;
						_db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails));
						await _db.SaveChangesAsync();
					}
					else
					{
						//update count in cart details
						cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
						cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
						cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
						_db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails));
						await _db.SaveChangesAsync();
					}
				}
				_responseDto.Result = cartDto;
			}
			catch (Exception ex)
			{
				_responseDto.Message = ex.Message.ToString();
				_responseDto.IsSuccess = false;
			}
			return _responseDto;
		}

		[HttpDelete("RemoveCart")]
		public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
		{
			try
			{
				CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);

				int totalCountOfCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetailsId).Count();

				_db.CartDetails.Remove(cartDetails);
				if (totalCountOfCartItem == 1)
				{
					var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

					_db.CartHeaders.Remove(cartHeaderToRemove);
				}
				await _db.SaveChangesAsync();

				_responseDto.Result = true;
			}
			catch (Exception ex)
			{
				_responseDto.Message = ex.Message.ToString();
				_responseDto.IsSuccess = false;
			}
			return _responseDto;
		}
	}
}
