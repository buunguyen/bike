namespace Bike.Interpreter.Builtin
{
    using System.Collections;
    using System.Collections.Generic;

    public class BikeArray : BikeObject
    {
        public List<object> Value
        {
            get
            {
                return (List<object>)Members[InterpreterHelper.SpecialSuffix + "native"];
            }
        }

        public BikeArray()
            : base(InterpretationContext.ArrayBase)
        {
            Members[InterpreterHelper.SpecialSuffix + "native"] = new List<object>();
        }

        public override IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Value.Count; i++)
            {
                yield return this[i];
            }
        }

        #region Implementation of BikeObject
        internal object this[int index]
        {
            get
            {
                return Value[index];
            }
            set
            {
                for (int i = Value.Count; i <= index; i++)
                    Value.Add(null);
                Value[index] = value;
            }
        }

        public override void Assign(string name, object value)
        {
            int index;
            if (int.TryParse(name, out index))
            {
                this[index] = value;
            }
            else
            {
                base.Assign(name, value);
            }
        }

        public override object Resolve(string name)
        {
            int index;
            return int.TryParse(name, out index)
                ? this[index]
                : base.Resolve(name);
        }

        public override bool Exist(string name)
        {
            int index;
            if (int.TryParse(name, out index) && index < Value.Count)
                return true;
            return base.Exist(name);
        }

        public override bool OwnExist(string name)
        {
            int index;
            if (int.TryParse(name, out index) && index < Value.Count)
                return true;
            return Members.ContainsKey(name);
        }
        #endregion
    }
}
