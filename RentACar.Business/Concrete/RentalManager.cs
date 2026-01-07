using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Concrete
{
    public class RentalManager : IRentalService
    {
        IRentalDal _rentalDal;
        ICarDal _carDal;

        public RentalManager(IRentalDal rentalDal, ICarDal carDal)
        {
            _rentalDal = rentalDal;
            _carDal = carDal;
        }

        public IResult Add(Rental rental)
        {
            // Check if car exists
            var car = _carDal.Get(c => c.CarId == rental.CarId);
            if (car == null)
            {
                return new ErrorResult("Car not found");
            }

            // Check if car is available for the requested dates
            var existingRentals = _rentalDal.GetAll(r => 
                r.CarId == rental.CarId && 
                r.ReturnDate == null);

            if (existingRentals.Any())
            {
                return new ErrorResult("Car is not available. It has not been returned yet");
            }

            // Check for overlapping rentals
            var overlappingRentals = _rentalDal.GetAll(r => 
                r.CarId == rental.CarId && 
                r.ReturnDate != null &&
                (
                    (rental.RentDate >= r.RentDate && rental.RentDate <= r.ReturnDate) ||
                    (rental.ReturnDate != null && rental.ReturnDate >= r.RentDate && rental.ReturnDate <= r.ReturnDate)
                )
            );

            if (overlappingRentals.Any())
            {
                return new ErrorResult("Car is not available for the selected dates");
            }

            _rentalDal.Add(rental);
            return new SuccessResult("Rental added successfully");
        }

        public IResult Delete(Rental rental)
        {
            _rentalDal.Delete(rental);
            return new SuccessResult("Rental deleted");
        }

        public IDataResult<List<Rental>> GetAll()
        {
            return new SuccessDataResult<List<Rental>>(_rentalDal.GetAll(), "Rentals listed");
        }

        // Add this missing method
        public IDataResult<Rental> GetById(int rentalId)
        {
            var rental = _rentalDal.Get(r => r.RentalId == rentalId);
            if (rental == null)
            {
                return new ErrorDataResult<Rental>("Rental not found");
            }
            return new SuccessDataResult<Rental>(rental);
        }

        public IResult Update(Rental rental)
        {
            // Validate return date if provided
            if (rental.ReturnDate != null && rental.ReturnDate < rental.RentDate)
            {
                return new ErrorResult("Return date cannot be earlier than rent date");
            }

            _rentalDal.Update(rental);
            return new SuccessResult("Rental updated");
        }
    }
}