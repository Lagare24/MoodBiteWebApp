using MoodBite.Contract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MoodBite.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        private DbContext _db;
        private DbSet<T> _table;

        public BaseRepository()
        {
            _db = new MoodBiteEntities();
            _table = _db.Set<T>();
        }

        public DbSet<T> Table
        {
            get
            {
                return _table;
            }
        }

        public ErrorCode Create(T t)
        {
            try
            {
                _table.Add(t);
                _db.SaveChanges();
                return ErrorCode.Success;
            }
            catch (Exception)
            {
                return ErrorCode.Error;
            }
        }

        public ErrorCode Delete(object id)
        {
            try
            {
                var user = Get(id);
                _table.Remove(user);
                _db.SaveChanges();
                return ErrorCode.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ErrorCode.Error;
            }
        }

        public T Get(object id)
        {
            return _table.Find(id);
        }

        public List<T> GetAll()
        {
            return _table.ToList();
        }

        public ErrorCode Update(object id, T t)
        {
            try
            {
                var user = Get(id);
                _db.Entry(user).CurrentValues.SetValues(t);
                _db.SaveChanges();
                return ErrorCode.Success;
            }
            catch (Exception)
            {
                return ErrorCode.Error;
            }
        }
    }
}