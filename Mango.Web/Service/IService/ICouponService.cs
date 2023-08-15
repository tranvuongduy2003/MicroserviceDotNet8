﻿using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string code);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}
