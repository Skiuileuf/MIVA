using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MivaForms
{
    public class ValuesTuple<T>
    {
        public T Pech;
        public T Prom;
        public T Stan;

        public ValuesTuple(T pech, T prom, T stan)
        {
            Pech = pech;
            Prom = prom;
            Stan = stan;
        }

        public ValuesTuple()
        {
            Pech = default;
            Prom = default;
            Stan = default;
        }

        public override string ToString()
        {
            return $"Pech: {Pech}, Prom: {Prom}, Stan: {Stan}";
        }

        public static ValuesTuple<T> operator +(ValuesTuple<T> a, ValuesTuple<T> b)
        {
            return new ValuesTuple<T>((dynamic)a.Pech + (dynamic)b.Pech, (dynamic)a.Prom + (dynamic)b.Prom, (dynamic)a.Stan + (dynamic)b.Stan);
        }

        public T Sum()
        {
            return (dynamic)Pech + (dynamic)Prom + (dynamic)Stan;
        }

        public static ValuesTuple<T> operator -(ValuesTuple<T> a, ValuesTuple<T> b)
        {
            return new ValuesTuple<T>((dynamic)a.Pech - (dynamic)b.Pech, (dynamic)a.Prom - (dynamic)b.Prom, (dynamic)a.Stan - (dynamic)b.Stan);
        }

        public static ValuesTuple<T> operator *(ValuesTuple<T> a, ValuesTuple<T> b)
        {
            return new ValuesTuple<T>((dynamic)a.Pech * (dynamic)b.Pech, (dynamic)a.Prom * (dynamic)b.Prom, (dynamic)a.Stan * (dynamic)b.Stan);
        }

        public static ValuesTuple<T> operator /(ValuesTuple<T> a, ValuesTuple<T> b)
        {
            return new ValuesTuple<T>((dynamic)a.Pech / (dynamic)b.Pech, (dynamic)a.Prom / (dynamic)b.Prom, (dynamic)a.Stan / (dynamic)b.Stan);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (ValuesTuple<T>)obj;
            return Pech.Equals(other.Pech) && Prom.Equals(other.Prom) && Stan.Equals(other.Stan);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Pech, Prom, Stan);
        }

    }
}
