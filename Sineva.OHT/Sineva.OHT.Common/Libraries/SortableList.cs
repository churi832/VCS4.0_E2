using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sineva.OHT.Common
{
    public class SortableList<T> : List<T>
    {
        private string[] _FieldList;
        private string _Field;
        private object[] _ValueList;

        public void Sort(params string[] fields) => this.Sort((IComparer<T>)new ObjectComparer<T>(fields));

        public void Sort(string[] fields, bool stringToInt) => this.Sort((IComparer<T>)new ObjectComparer<T>(fields, stringToInt));

        public int Sort(string[] fields, bool[] descending)
        {
            if (fields.Length != descending.Length)
                return -1;
            this.Sort((IComparer<T>)new ObjectComparer<T>(fields, descending));
            return 0;
        }

        public int Sort(string[] fields, bool[] descending, bool stringToInt)
        {
            if (fields.Length != descending.Length)
                return -1;
            this.Sort((IComparer<T>)new ObjectComparer<T>(fields, descending, stringToInt));
            return 0;
        }

        public T Find(string[] fields, object[] values)
        {
            if (fields.Length != values.Length)
                return default(T);
            this._FieldList = fields;
            this._ValueList = values;
            return this.Find(new Predicate<T>(this.Filter<T>));
        }

        public SortableList<T> FindAll(string[] fields, object[] values)
        {
            if (fields.Length != values.Length)
                return (SortableList<T>)null;
            this._FieldList = fields;
            this._ValueList = values;
            List<T> all = this.FindAll(new Predicate<T>(this.Filter<T>));
            SortableList<T> sortableList = new SortableList<T>();
            for (int index = 0; index < all.Count; ++index)
                sortableList.Add(all[index]);
            return sortableList;
        }

        public SortableList<T> FindAll(string[] fields, object[] values, bool bNot)
        {
            if (fields.Length != values.Length)
                return (SortableList<T>)null;
            this._FieldList = fields;
            this._ValueList = values;
            List<T> objList = !bNot ? this.FindAll(new Predicate<T>(this.Filter<T>)) : this.FindAll(new Predicate<T>(this.FilterNot<T>));
            SortableList<T> sortableList = new SortableList<T>();
            for (int index = 0; index < objList.Count; ++index)
                sortableList.Add(objList[index]);
            return sortableList;
        }

        public SortableList<T> FindAll(string fields, object[] values)
        {
            this._Field = fields;
            this._ValueList = values;
            List<T> all = this.FindAll(new Predicate<T>(this.FilterOR<T>));
            SortableList<T> sortableList = new SortableList<T>();
            for (int index = 0; index < all.Count; ++index)
                sortableList.Add(all[index]);
            return sortableList;
        }

        public SortableList<T> FindAll(string fields, object[] values, bool not)
        {
            this._Field = fields;
            this._ValueList = values;
            List<T> objList = !not ? this.FindAll(new Predicate<T>(this.FilterOR<T>)) : this.FindAll(new Predicate<T>(this.FilterORNot<T>));
            SortableList<T> sortableList = new SortableList<T>();
            for (int index = 0; index < objList.Count; ++index)
                sortableList.Add(objList[index]);
            return sortableList;
        }

        private bool Filter<K>(K x)
        {
            Type type = x.GetType();
            for (int index = 0; index < this._FieldList.Length; ++index)
            {
                PropertyInfo property = type.GetProperty(this._FieldList[index]);
                if (property == (PropertyInfo)null)
                    return false;
                IComparable comparable = (IComparable)property.GetValue((object)x, (object[])null);
                if (comparable == null || comparable.CompareTo(this._ValueList[index]) != 0)
                    return false;
            }
            return true;
        }

        private bool FilterNot<K>(K x)
        {
            Type type = x.GetType();
            for (int index = 0; index < this._FieldList.Length; ++index)
            {
                PropertyInfo property = type.GetProperty(this._FieldList[index]);
                if (property == (PropertyInfo)null)
                    return false;
                IComparable comparable = (IComparable)property.GetValue((object)x, (object[])null);
                if (comparable == null)
                    return false;
                if (comparable.CompareTo(this._ValueList[index]) != 0)
                    return true;
            }
            return false;
        }

        private bool FilterOR<K>(K x)
        {
            PropertyInfo property = x.GetType().GetProperty(this._Field);
            if (property == (PropertyInfo)null)
                return false;
            IComparable comparable = (IComparable)property.GetValue((object)x, (object[])null);
            if (comparable == null)
                return false;
            for (int index = 0; index < this._ValueList.Length; ++index)
            {
                if (comparable.CompareTo(this._ValueList[index]) == 0)
                    return true;
            }
            return false;
        }

        private bool FilterORNot<K>(K x)
        {
            PropertyInfo property = x.GetType().GetProperty(this._Field);
            if (property == (PropertyInfo)null)
                return false;
            IComparable comparable = (IComparable)property.GetValue((object)x, (object[])null);
            if (comparable == null)
                return false;
            for (int index = 0; index < this._ValueList.Length; ++index)
            {
                if (comparable.CompareTo(this._ValueList[index]) != 0)
                    return true;
            }
            return false;
        }
    }

    public class ObjectComparer<T> : IComparer<T>
    {
        protected string[] _FieldList;
        protected bool[] _DescendingList;
        protected bool _StringToInt;

        public int Compare(T x, T y)
        {
            Type type1 = x.GetType();
            Type type2 = y.GetType();
            for (int index = 0; index < this._FieldList.Length; ++index)
            {
                PropertyInfo property1 = type1.GetProperty(this._FieldList[index]);
                PropertyInfo property2 = type2.GetProperty(this._FieldList[index]);
                if (!(property1 == (PropertyInfo)null) && !(property2 == (PropertyInfo)null))
                {
                    IComparable comparable = (IComparable)property1.GetValue((object)x, (object[])null);
                    object obj = property2.GetValue((object)y, (object[])null);
                    int num;
                    if (comparable == null && obj == null)
                        num = 0;
                    else if (comparable == null && obj != null)
                        num = -1;
                    else if (this._StringToInt)
                    {
                        int result1 = 0;
                        int result2 = 0;
                        num = !int.TryParse(comparable.ToString(), out result1) || !int.TryParse(obj.ToString(), out result2) ? comparable.CompareTo(obj) : result1.CompareTo(result2);
                    }
                    else
                        num = comparable.CompareTo(obj);
                    if (num != 0)
                        return this._DescendingList[index] ? -num : num;
                }
            }
            return 0;
        }

        public ObjectComparer(params string[] fields)
          : this(fields, new bool[fields.Length])
        {
        }

        public ObjectComparer(string[] fields, bool stringToInt)
          : this(fields, new bool[fields.Length])
        {
            this._StringToInt = stringToInt;
        }

        public ObjectComparer(string[] fields, bool[] descending)
        {
            this._FieldList = fields;
            this._DescendingList = descending;
        }

        public ObjectComparer(string[] fields, bool[] descending, bool stringToInt)
        {
            this._FieldList = fields;
            this._DescendingList = descending;
            this._StringToInt = stringToInt;
        }
    }
}
