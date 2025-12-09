using RentACar.Core.DataAccess;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.DataAccess.Abstract
{
    public interface ICarDal : IEntityRepository<Car>
    {
        List<CarDetailDto> GetCarDetails();
        List<CarDetailDto> GetCarDetail(int carId);
    }
}