using Microsoft.AspNetCore.Mvc;
using Model1.Models;

namespace Model1.Controllers
{
    [ApiController]
    public class RestaurantController(MapDbContext db) : ControllerBase
    {
        [HttpGet("system/api/restaurant/list")]
        public ApiResult List(int pageNun = 1
            ,int pageSize = 10
            ,string? restaurantName = null
            ,string? restaurantType = null
            )
        {
            var q = db.Restaurants.AsQueryable(); //ใช่เชื่อมต่อกับตาราง Restaurant
            if (!string.IsNullOrEmpty(restaurantName))  // กรองเฉพาะร้านที่มีชื่อตรงกับคำค้นหา (คล้ายคำสั่ง LIKE '%ชื่อร้าน%' ใน SQL)
                q = q.Where(x => x.RestaurantName!.Contains(restaurantName)); // ตรวจสอบว่ามีการส่ง "ประเภทร้านอาหาร" มาหรือไม่
            if (!string.IsNullOrEmpty(restaurantType))// กรองเฉพาะร้านที่มีประเภทตรงกันเป๊ะๆ (คล้ายคำสั่ง = 'ประเภทร้าน' ใน SQL)
                q = q.Where(x => x.RestaurantType == restaurantType);// ส่งผลลัพธ์กลับไปว่าทำงานสำเร็จ
            int total = q.Count();
            var rows = q.OrderBy(x => x.RestaurantId)
            .Skip((pageNun - 1) * pageSize)
            .Take(pageSize)
            .ToList()
            .Select(x => new {
                x.RestaurantId,
                x.RestaurantName,
                x.Address,
                x.Phone,
                rating = x.Rating.ToString(),
                x.PriceRange,
                x.RestaurantType,
                x.ImageUrl,
                x.Description,
                distance = x.Distance.ToString(),
                businessType = "restaurant"
            });
            
            return ApiResult.Ok(new { total, rows });
        } 

        [HttpGet("system/api/restaurant/{restaurantId}")]
        public ApiResult Detail(int restaurantId)
        {
            var x = db.Restaurants.FirstOrDefault(r => r.RestaurantId == restaurantId);
            if (x == null)
                return ApiResult.Fail(404, "Restaurant not found");
            return ApiResult.Ok(new
            {
                x.BusinessHours,
                x.RemainingParkingSpaces,
                x.RestaurantId,
                x.RestaurantName,
                x.Address,
                x.Phone,
                rating = x.Rating.ToString(),
                x.PriceRange,
                x.RestaurantType,
                x.ImageUrl,
                x.Description,
                distance = x.Distance.ToString(),
                businessType = "restaurant"
            });
        } 
    }
}
    

