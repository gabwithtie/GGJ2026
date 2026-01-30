using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GabUnity.Modules.TrackedVariable
{
    [Serializable]
    public class TrackedVariable<T> where T : struct
    {
        T cached_value;

        private readonly Action<T> OnChange;
        private readonly Func<T> GetValueFunction;

        public TrackedVariable(Action<T> onChange, Func<T> getValueFunction)
        {
            OnChange = onChange;
            GetValueFunction = getValueFunction;
        }

        public void CheckChange()
        {
            T updatedvalue = GetValueFunction();
            if (updatedvalue.Equals(cached_value) == false)
            {
                cached_value = updatedvalue;
                OnChange(cached_value);
            }
        }
    }
}
