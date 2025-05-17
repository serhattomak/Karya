using Karya.Domain.Entities;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;

namespace Karya.Persistence.Repositories;

public class ContactPageRepository(AppIdentityDbContext context) : EfRepository<ContactPage>(context), IContactPageRepository
{

}