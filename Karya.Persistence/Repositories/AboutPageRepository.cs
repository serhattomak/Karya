using Karya.Domain.Entities;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;

namespace Karya.Persistence.Repositories;

public class AboutPageRepository(AppIdentityDbContext context) : EfRepository<AboutPage>(context), IAboutPageRepository
{

}