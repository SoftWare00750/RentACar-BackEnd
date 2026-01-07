using RentACar.Business.Abstract;
using RentACar.Core.Utilities.Results;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.Business.Concrete
{
    public class BrandManager : IBrandService
    {
        IBrandDal _brandDal;

        public BrandManager(IBrandDal brandDal)
        {
            _brandDal = brandDal;
        }

        public IResult Add(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.BrandName))
            {
                return new ErrorResult("Brand name is required");
            }

            var existingBrand = _brandDal.Get(b => b.BrandName.ToLower() == brand.BrandName.ToLower());
            if (existingBrand != null)
            {
                return new ErrorResult("Brand already exists");
            }

            _brandDal.Add(brand);
            return new SuccessResult("Brand added");
        }

        public IResult Delete(Brand brand)
        {
            _brandDal.Delete(brand);
            return new SuccessResult("Brand deleted");
        }

        public IDataResult<List<Brand>> GetAll()
        {
            return new SuccessDataResult<List<Brand>>(_brandDal.GetAll(), "Brands listed");
        }

        public IDataResult<Brand> GetById(int brandId)
        {
            var brand = _brandDal.Get(b => b.BrandId == brandId);
            if (brand == null)
            {
                return new ErrorDataResult<Brand>("Brand not found");
            }
            return new SuccessDataResult<Brand>(brand);
        }

        public IResult Update(Brand brand)
        {
            if (string.IsNullOrWhiteSpace(brand.BrandName))
            {
                return new ErrorResult("Brand name is required");
            }

            var existingBrand = _brandDal.Get(b => b.BrandName.ToLower() == brand.BrandName.ToLower() && b.BrandId != brand.BrandId);
            if (existingBrand != null)
            {
                return new ErrorResult("Another brand with this name already exists");
            }

            _brandDal.Update(brand);
            return new SuccessResult("Brand updated");
        }
    }
}