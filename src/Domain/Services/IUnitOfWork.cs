using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Service
{
    public interface IUnitOfWork
    {
        void Save();
    }
}
