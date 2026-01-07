using RentACar.Core.DataAccess.EntityFramework;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, RentACarContext>, ICarDal
    {
        public List<CarDetailDto> GetCarDetails()
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from c in context.Cars
                             join b in context.Brands
                                 on c.BrandId equals b.BrandId
                             join col in context.Colors
                                 on c.ColorId equals col.ColorId
                             select new CarDetailDto
                             {
                                 CarId = c.CarId,
                                 BrandId = c.BrandId,
                                 ColorId = c.ColorId,
                                 BrandName = b.BrandName,
                                 ColorName = col.ColorName,
                                 ModelYear = c.ModelYear,
                                 DailyPrice = c.DailyPrice,
                                 Description = c.Description
                             };
                return result.ToList();
            }
        }

        public List<CarDetailDto> GetCarDetail(int carId)
        {
            using (RentACarContext context = new RentACarContext())
            {
                var result = from c in context.Cars
                             join b in context.Brands
                                 on c.BrandId equals b.BrandId
                             join col in context.Colors
                                 on c.ColorId equals col.ColorId
                             where c.CarId == carId
                             select new CarDetailDto
                             {
                                 CarId = c.CarId,
                                 BrandId = c.BrandId,
                                 ColorId = c.ColorId,
                                 BrandName = b.BrandName,
                                 ColorName = col.ColorName,
                                 ModelYear = c.ModelYear,
                                 DailyPrice = c.DailyPrice,
                                 Description = c.Description
                             };
                return result.ToList();
            }
        }
    }
}