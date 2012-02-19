using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omu.ValueInjecter;
using System.Collections;

namespace cf.Entities
{
    public class SimpleTypeCloneInjection : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            return c.SourceProp.Name == c.TargetProp.Name && c.SourceProp.Value != null;
        }     

        protected override object SetValue(ConventionInfo c)
        {
            //for value types and string just return the value as is
            if (c.SourceProp.Type.IsValueType || c.SourceProp.Type == typeof(string))
                return c.SourceProp.Value;
        
            //for simple object types create a new instance and apply the clone injection on it
                        
            return null;
        }
    }

}
