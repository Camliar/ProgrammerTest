using Yitter.IdGenerator;

namespace ProgrammerTest.Order.WebApi.Common.Utils
{
    public static class IdGenerator
    {

        static IdGenerator()
        {
            var options = new IdGeneratorOptions(0);
            YitIdHelper.SetIdGenerator(options);
        }

        public static long NextId()
        {
            return YitIdHelper.NextId();
        }
    }
}
