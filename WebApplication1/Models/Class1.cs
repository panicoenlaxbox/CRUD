using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            return _repository.GetAll<TKey>(page, pageSize, predicates, orderyBy);
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
        Customer Find(int id);
        PaginatedResult<CustomerDto> GetAll(int page, int pageSize, string orderBy, string name);
        void Add(Customer customer);
        void Update(Customer customer);
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

        public Customer Find(int id)
        {
            return _customersRepository.Find(id);
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

        public void Add(Customer customer)
        {
            _customersRepository.Add(customer);
            _unitOfWork.SaveChanges();
        }

        public void Update(Customer customer)
        {
            _customersRepository.Update(customer);
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

    /// <summary>
    /// Dynamic SQL-like Linq OrderBy Extension
    /// </summary>
    /// <remarks>http://aonnull.blogspot.com.es/2010/08/dynamic-sql-like-linq-orderby-extension.html</remarks>
    public static class OrderByExtension
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy<T>(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            string[] props = orderByInfo.PropertyName.Split('.');
            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            string methodName = String.Empty;

            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "ThenBy";
                else
                    methodName = "ThenByDescending";
            }
            else
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "OrderBy";
                else
                    methodName = "OrderByDescending";
            }

            //TODO: apply caching to the generic methodsinfos?
            return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                        && method.IsGenericMethodDefinition
                        && method.GetGenericArguments().Length == 2
                        && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), type)
                .Invoke(null, new object[] { collection, lambda });
        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(String.Format("Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC", item));

                string prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException("Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                SortDirection dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase) ? SortDirection.Descending : SortDirection.Ascending);

                yield return new OrderByInfo() { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }

        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }
}