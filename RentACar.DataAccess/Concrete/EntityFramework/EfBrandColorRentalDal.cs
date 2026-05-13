// ── EfBrandDal.cs ─────────────────────────────────────────────────────────────
using RentACar.Core.DataAccess.EntityFramework;
using RentACar.DataAccess.Abstract;
using RentACar.Entities.Concrete;

namespace RentACar.DataAccess.Concrete.EntityFramework
{
    public class EfBrandDal : EfEntityRepositoryBase<Brand, RentACarContext>, IBrandDal
    {
        public EfBrandDal(RentACarContext context) : base(context) { }
    }

    public class EfColorDal : EfEntityRepositoryBase<Color, RentACarContext>, IColorDal
    {
        public EfColorDal(RentACarContext context) : base(context) { }
    }

    public class EfRentalDal : EfEntityRepositoryBase<Rental, RentACarContext>, IRentalDal
    {
        public EfRentalDal(RentACarContext context) : base(context) { }
    }
}