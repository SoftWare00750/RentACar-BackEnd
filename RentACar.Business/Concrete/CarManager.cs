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
            if (car.DailyPrice <= 0)
            {
                return new ErrorResult("Daily price must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(car.Description))
            {
                return new ErrorResult("Description is required");
            }
            
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
            var carDetail = _carDal.GetCarDetail(carId);
            if (carDetail == null || carDetail.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>("Car detail not found");
            }
            return new SuccessDataResult<List<CarDetailDto>>(carDetail, "Car detail retrieved");
        }

        public IDataResult<List<CarDetailDto>> GetCarsByBrandId(int brandId)
        {
            var carDetails = _carDal.GetCarDetails();
            var filteredCars = carDetails.Where(c => c.BrandId == brandId).ToList();
            
            if (filteredCars.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>("No cars found for this brand");
            }
            
            return new SuccessDataResult<List<CarDetailDto>>(filteredCars, "Cars filtered by brand");
        }

        public IDataResult<List<CarDetailDto>> GetCarsByColorId(int colorId)
        {
            var carDetails = _carDal.GetCarDetails();
            var filteredCars = carDetails.Where(c => c.ColorId == colorId).ToList();
            
            if (filteredCars.Count == 0)
            {
                return new ErrorDataResult<List<CarDetailDto>>("No cars found for this color");
            }
            
            return new SuccessDataResult<List<CarDetailDto>>(filteredCars, "Cars filtered by color");
        }

        public IResult Update(Car car)
        {
            if (car.DailyPrice <= 0)
            {
                return new ErrorResult("Daily price must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(car.Description))
            {
                return new ErrorResult("Description is required");
            }
            
            _carDal.Update(car);
            return new SuccessResult("Car updated");
        }
    }
}