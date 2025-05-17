using Karya.Domain.Entities;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;

namespace Karya.Persistence.Repositories;

public class HomePageRepository(AppIdentityDbContext context) : EfRepository<HomePage>(context), IHomePageRepository
{

}