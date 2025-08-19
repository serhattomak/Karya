using Karya.Domain.Entities;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;

namespace Karya.Persistence.Repositories;

public class ContactRepository(AppDbContext context) : EfRepository<Contact>(context), IContactRepository
{

}