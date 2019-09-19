
namespace Microsoft.Extensions.DependencyInjection
{


    public static class GetTypeInfoExtensions
    {

            
        public static bool HasDefaultValue(this System.Reflection.ParameterInfo pi)
        {
            return false;
        }


        public static System.Type GetTypeInfo(this System.Type t)
        {
            return t;
        }


        public static bool IsConstructedGenericType(this System.Type t)
        {
            return false;
        }


        

        public static System.Type[] GenericTypeArguments(this System.Type t)
        {
            if (t.IsGenericType && !t.IsGenericTypeDefinition)
            {
                return t.GetGenericArguments();
            }
            else
            {
                return System.Type.EmptyTypes;
            }

        }


        public static System.Collections.Generic.IEnumerable<System.Reflection.ConstructorInfo> DeclaredConstructors(this System.Type t)
        {
            return null;
        }
        



    }


}
