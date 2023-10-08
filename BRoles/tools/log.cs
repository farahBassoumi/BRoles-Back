

using log4net;
using log4net.Config;
using System.Reflection;

namespace BRoles.tools
{
    public class Log
    {
        private static readonly ILog logger =
           LogManager.GetLogger(typeof(Log));

        static Log()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public static void WriteLine(string txt, int i = 0)
        {
            switch (i)
            {
                // Lors du debug
                case 1:
                    logger.Debug(txt);
                    break;

                // Exception ou Erreur
                case 2:
                    logger.Warn(txt);
                    break;

                // Cas ordinaire
                default:
                    logger.Info(txt);
                    break;
            }
        }
        public static void WriteSENSITIVESLine(string txt, IConfiguration configuration)
        {

            int LOG_SENSITIVES_DATA = 0;
            try
            {
                _ = int.TryParse(configuration["LOG_SENSITIVES_DATA"].ToString(), out LOG_SENSITIVES_DATA);
            }
            catch (Exception) { }
            if (LOG_SENSITIVES_DATA == 1)
            {
                logger.Info("ONLY FOR TESTS:" + txt);
            }
        }
    }
}

