namespace Utils
{
	public delegate void VoidOp();
    public delegate int IntOp(int i);
	
    public class Event
    {
        public static VoidOp voidDelegate;
        public static void AssignDelegate(VoidOp op)
        {
            voidDelegate = op;
        }

        public event IntOp intEvent;
        public void AddEvent(IntOp op)
        {
            intEvent += op;
        }
        public int TriggerEvent(int arg)
        {
            return intEvent(arg);
        }
	}
	
	public class Generic
	{
		public static T Max<T>(T t1, T t2) where T : System.IComparable 
		{	
			 return t1.CompareTo(t2) > 0 ? t1 : t2;
		}
		
		public System.Type GetType<T>() 
		{
			return typeof(T);
		}
	}
}