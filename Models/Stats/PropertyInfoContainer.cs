using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Models.Stats {
    public class PropertyInfoContainer {
        public PropertyInfo Property;
        public string Name => Property.Name;
        public string Key => Name.ToUpperInvariant();

        private MethodInfo? GetMethod { get; set; }
        private MethodInfo? SetMethod { get; set; }

        public bool CanWrite => Property.CanWrite && SetMethod != null && !SetMethod.IsPrivate;
        public bool IsStatic => GetMethod != null && GetMethod.IsStatic;
        public Type PropertyType => Property.PropertyType;

        public object? GetValue(object? instance) => Property.GetValue(instance);
        public void SetValue(object? instance, object? value) => Property.SetValue(instance, value);

        public PropertyInfoContainer(PropertyInfo property) {
            Property = property;
            GetMethod = property.GetGetMethod(false);
            SetMethod = property.GetSetMethod(false);
        }

        public override string ToString() {
            return Name;
        }
    }
}
