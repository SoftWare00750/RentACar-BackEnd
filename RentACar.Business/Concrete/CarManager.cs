using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;
using RentACar.Entities.DTOs;

namespace RentACar.Business.Concrete
{
    public class CarManager : ICarService
    {
        ICarDal _carDal;

        public CarManager(ICarDal carDal)
        {
            _carDal = carDal;
        }

        public IResult Add(Car car)
        {
            _carDal.Add(car);
            return new SuccessResult("Car added");
        }

        public IResult Delete(Car car)
        {
            _carDal.Delete(car);
            return new SuccessResult("Car deleted");
        }

        public IDataResult<List<Car>> GetAll()
        {
            return new SuccessDataResult<List<Car>>(_carDal.GetAll(), "Cars listed");
        }

        public IDataResult<Car> GetById(int carId)
        {
            var car = _carDal.Get(c => c.CarId == carId);
            if (car == null)
            {
                return new ErrorDataResult<Car>("Car not found");
            }
            return new SuccessDataResult<Car>(car);
        }

        public IDataResult<List<CarDetailDto>> GetCarDetails()
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetails(), "Car details listed");
        }

        public IDataResult<List<CarDetailDto>> GetCarDetail(int carId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(_carDal.GetCarDetail(carId), "Car detail retrieved");
        }

        public IDataResult<List<CarDetailDto>> GetCarsByBrandId(int brandId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(
                _carDal.GetCarDetails().Where(c => c.BrandName == _carDal.Get(x => x.CarId == brandId)?.BrandName).ToList(), 
                "Cars filtered by brand");
        }

        public IDataResult<List<CarDetailDto>> GetCarsByColorId(int colorId)
        {
            return new SuccessDataResult<List<CarDetailDto>>(
                _carDal.GetCarDetails().Where(c => c.ColorName == _carDal.Get(x => x.CarId == colorId)?.ColorName).ToList(), 
                "Cars filtered by color");
        }

        public IResult Update(Car car)
        {
            _carDal.Update(car);
            return new SuccessResult("Car updated");
        }
    }
}