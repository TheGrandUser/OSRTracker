using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace OSRTracker;

public static class SpanExtensions
{
   extension<T>(Span<T> self)
   {
      public V Sum<V>(Func<T, V> selector)
         where V : INumber<V>
      {
         V sum = V.Zero;

         for (var i = 0; i < self.Length; i++)
         {
            sum += selector(self[i]);
         }

         return sum;
      }
   }
}
