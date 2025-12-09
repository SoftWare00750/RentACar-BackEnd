using RentACar.Core.Utilities.Results;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Abstract
{
    public interface IRentalService
    {
        IDataResult<List<Rental>> GetAll();
        IResult Add(Rental rental);
        IResult Update(Rental rental);
        IResult Delete(Rental rental);
    }
}