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
            if (string.IsNullOrWhiteSpace(color.ColorName))
            {
                return new ErrorResult("Color name is required");
            }

            var existingColor = _colorDal.Get(c => c.ColorName.ToLower() == color.ColorName.ToLower());
            if (existingColor != null)
            {
                return new ErrorResult("Color already exists");
            }

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
            if (string.IsNullOrWhiteSpace(color.ColorName))
            {
                return new ErrorResult("Color name is required");
            }

            var existingColor = _colorDal.Get(c => c.ColorName.ToLower() == color.ColorName.ToLower() && c.ColorId != color.ColorId);
            if (existingColor != null)
            {
                return new ErrorResult("Another color with this name already exists");
            }

            _colorDal.Update(color);
            return new SuccessResult("Color updated");
        }
    }
}