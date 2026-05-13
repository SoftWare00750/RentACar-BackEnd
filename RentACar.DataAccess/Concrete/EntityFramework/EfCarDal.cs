using RentACar.Core.DataAccess.EntityFramework;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class EfCarDal : EfEntityRepositoryBase<Car, RentACarContext>, ICarDal
    {
        private readonly RentACarContext _context;

        public EfCarDal(RentACarContext context) : base(context)
        {
            _context = context;
        }

        public List<CarDetailDto> GetCarDetails()
        {
            return (from c in _context.Cars
                    join b   in _context.Brands on c.BrandId  equals b.BrandId
                    join col in _context.Colors on c.ColorId  equals col.ColorId
                    select new CarDetailDto
                    {
                        CarId       = c.CarId,
                        BrandId     = c.BrandId,
                        ColorId     = c.ColorId,
                        CarName     = (c.CarName != null && c.CarName != "")
                                          ? c.CarName
                                          : b.BrandName + " " + c.ModelYear,
                        BrandName   = b.BrandName,
                        ColorName   = col.ColorName,
                        ModelYear   = c.ModelYear,
                        DailyPrice  = c.DailyPrice,
                        Description = c.Description,
                        ImagePath   = "/images/default-car.jpg"
                    }).ToList();
        }

        public List<CarDetailDto> GetCarDetail(int carId)
        {
            return (from c in _context.Cars
                    join b   in _context.Brands on c.BrandId  equals b.BrandId
                    join col in _context.Colors on c.ColorId  equals col.ColorId
                    where c.CarId == carId
                    select new CarDetailDto
                    {
                        CarId       = c.CarId,
                        BrandId     = c.BrandId,
                        ColorId     = c.ColorId,
                        CarName     = (c.CarName != null && c.CarName != "")
                                          ? c.CarName
                                          : b.BrandName + " " + c.ModelYear,
                        BrandName   = b.BrandName,
                        ColorName   = col.ColorName,
                        ModelYear   = c.ModelYear,
                        DailyPrice  = c.DailyPrice,
                        Description = c.Description,
                        ImagePath   = "/images/default-car.jpg"
                    }).ToList();
        }
    }
}