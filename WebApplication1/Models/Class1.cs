using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;

namespace WebApplication1.Models
{
    public interface IRepository<T> where T : class
    {
        T Find(params object[] keyValues);
        PaginatedResult<T> GetAll<TKey>(int page, int pageSize, IEnumerable<Expression<Func<T, bool>>> predicates, Expression<Func<T, TKey>> orderBy);
        PaginatedResult<T> GetAll(int page, int pageSize, IEnumerable<Expression<Func<T, bool>>> predicates, string orderBy);
        void Add(T entity);
        void Update(T entity);
        void Delete(params object[] keyValues);
    }

    public class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _context;
        private readonly DbSet<T> _set;

        public EfRepository(DbContext context)
        {
            _context = context;
            _set = context.Set<T>();
        }

        public T Find(params object[] keyValues)
        {
            return _set.Find(keyValues);
        }

        public PaginatedResult<T> GetAll<TKey>(int page, int pageSize, IEnumerable<Expression<Func<T, bool>>> predicates, Expression<Func<T, TKey>> orderBy)
        {
            IQueryable<T> query = _set;
            if (predicates != null)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            var totalSize = query.Count();
            query = query.OrderBy(orderBy).Skip((page < 1 ? 0 : page - 1) * pageSize).Take(pageSize);
            System.Diagnostics.Debug.WriteLine(query.ToString());
            return new PaginatedResult<T>(page, pageSize, query.ToList(), totalSize);
        }

        public PaginatedResult<T> GetAll(int page, int pageSize, IEnumerable<Expression<Func<T, bool>>> predicates, string orderBy)
        {
            IQueryable<T> query = _set;
            if (predicates != null)
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            var totalSize = query.Count();
            query = query.OrderBy(orderBy).Skip((page < 1 ? 0 : page - 1) * pageSize).Take(pageSize);
            System.Diagnostics.Debug.WriteLine(query.ToString());
            return new PaginatedResult<T>(page, pageSize, query.ToList(), totalSize);
        }

        public void Add(T entity)
        {
            _set.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var entity = Find(keyValues);
            _set.Remove(entity);
        }
    }

    public interface ICustomersRepository
    {
        Customer Find(int id);
        PaginatedResult<Customer> GetAll<TKey>(int page, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, Expression<Func<Customer, TKey>> orderyBy);
        PaginatedResult<Customer> GetAll(int page, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, string orderBy);
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
    }

    public class EfCustomersRepository : ICustomersRepository
    {
        private readonly IRepository<Customer> _repository;

        public EfCustomersRepository(DbContext context)
        {
            _repository = new EfRepository<Customer>(context);
            
        }

        public Customer Find(int id)
        {
            return _repository.Find(id);
        }

        public PaginatedResult<Customer> GetAll<TKey>(int page, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, Expression<Func<Customer, TKey>> orderyBy)
        {
            return _repository.GetAll(page, pageSize, predicates, orderyBy);
        }

        public PaginatedResult<Customer> GetAll(int page, int pageSize, IEnumerable<Expression<Func<Customer, bool>>> predicates, string orderBy)
        {
            return _repository.GetAll(page, pageSize, predicates, orderBy);
        }

        public void Add(Customer customer)
        {
            _repository.Add(customer);
        }

        public void Update(Customer customer)
        {
            _repository.Update(customer);
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }
    }

    public interface ICustomersService
    {
        CustomerDto Find(int id);
        PaginatedResult<CustomerDto> GetAll(int page, int pageSize, string orderBy, string name);
        void Add(CustomerDto customer);
        void Update(CustomerDto customer);
        void Delete(int id);
    }

    public class CustomersService : ICustomersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomersRepository _customersRepository;

        public CustomersService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _customersRepository = _unitOfWork.Customers;
        }

        public CustomerDto Find(int id)
        {
            return Mapper.Map<Customer, CustomerDto>(_customersRepository.Find(id));
        }

        public PaginatedResult<CustomerDto> GetAll(int page, int pageSize, string orderBy, string name)
        {
            IList<Expression<Func<Customer, bool>>> predicates = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicates = new List<Expression<Func<Customer, bool>>>
                {
                    customer => customer.Name.Contains(name)
                };
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "Id";
            }
            var result = _customersRepository.GetAll(page, pageSize, predicates, orderBy);
            return new PaginatedResult<CustomerDto>(
                result.Page,
                result.PageSize,
                AutoMapper.Mapper.Map<IEnumerable<Customer>, IEnumerable<CustomerDto>>(result.Result),
                result.TotalSize);
        }

        public void Add(CustomerDto customer)
        {
            _customersRepository.Add(Mapper.Map<CustomerDto, Customer>(customer));
            _unitOfWork.SaveChanges();
        }

        public void Update(CustomerDto customer)
        {
            _customersRepository.Update(Mapper.Map<CustomerDto, Customer>(customer));
            _unitOfWork.SaveChanges();
        }

        public void Delete(int id)
        {
            _customersRepository.Delete(id);
            _unitOfWork.SaveChanges();
        }
    }

    public interface IUnitOfWork
    {
        void SaveChanges();
        ICustomersRepository Customers { get; }
    }

    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private ICustomersRepository _customers;

        public EfUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();

        }

        public ICustomersRepository Customers
        {
            get
            {
                return _customers ?? (_customers = new EfCustomersRepository(_context));
            }
        }
    }

    public class PaginatedResult<T>
    {
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public IEnumerable<T> Result { get; private set; }
        public int TotalSize { get; private set; }

        public PaginatedResult(int page, int pageSize, IEnumerable<T> result, int totalSize)
        {
            Page = page;
            PageSize = pageSize;
            Result = result;
            TotalSize = totalSize;
        }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}