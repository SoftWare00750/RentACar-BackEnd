using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Concrete
{
    public class ColorManager : IColorService
    {
        IColorDal _colorDal;

        public ColorManager(IColorDal colorDal)
        {
            _colorDal = colorDal;
        }

        public IResult Add(Color color)
        {
            _colorDal.Add(color);
            return new SuccessResult("Color added");
        }

        public IResult Delete(Color color)
        {
            _colorDal.Delete(color);
            return new SuccessResult("Color deleted");
        }

        public IDataResult<List<Color>> GetAll()
        {
            return new SuccessDataResult<List<Color>>(_colorDal.GetAll(), "Colors listed");
        }

        public IDataResult<Color> GetById(int colorId)
        {
            var color = _colorDal.Get(c => c.ColorId == colorId);
            if (color == null)
            {
                return new ErrorDataResult<Color>("Color not found");
            }
            return new SuccessDataResult<Color>(color);
        }

        public IResult Update(Color color)
        {
            _colorDal.Update(color);
            return new SuccessResult("Color updated");
        }
    }
}