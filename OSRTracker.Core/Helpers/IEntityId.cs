using System;
using System.Collections.Generic;
using System.Text;

namespace OSRTracker.Helpers;

public interface IEntityId<T> where T : IEntityId<T>
{
   static abstract T Create(int id);
   static abstract T Empty { get; }

   int Id { get; }
}
