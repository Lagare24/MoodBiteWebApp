using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoodBite.Contract
{
    public enum ErrorCode
    {
        Success,
        Error
    }
    interface IBaseRepository<T>
    {
        T Get(object id);
        List<T> GetAll();
        ErrorCode Create(T t);
        ErrorCode Update(object id, T t);
        ErrorCode Delete(object id);
    }
}
