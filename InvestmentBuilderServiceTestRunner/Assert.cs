using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentBuilderServiceTestRunner
{
    /// <summary>
    /// Custom exception raised if atest fails an assertion.
    /// </summary>
    internal class AssertionFailedException : Exception
    {
        /// <summary>
        /// Override message.
        /// </summary>
        public override string Message { get { return m_message; } }

        public AssertionFailedException(string message)
        {
            m_message = message;
        }

        private readonly string m_message;
    }

    /// <summary>
    /// Assert class used for testing outcomes.
    /// </summary>
    internal static class Assert
    {
        #region Public Methods

        /// <summary>
        /// Assert if result not true.
        /// </summary>
        public static void IsTrue(bool result, string description, string errorstring)
        {
            if(result)
            {
                logger.Info($"{description} PASSED");
            }
            else
            {
                throw new AssertionFailedException($"{description} FAILED. Expected true. was false. {errorstring}");
            }
        }

        /// <summary>
        /// Assert if not null
        /// </summary>
        public static void IsNull(object result, string description)
        {
            if (result == null)
            {
                logger.Info($"{description} PASSED");
            }
            else
            {
                throw new AssertionFailedException($"{description} FAILED. Expected null. was not null.");
            }
        }


        /// <summary>
        /// Assert if not equal.
        /// </summary>
        public static void AreEqual<T>(T expected, T actual, string description) where T : IEquatable<T>
        {
            if(expected.Equals(actual))
            {
                logger.Info($"{description} PASSED");
            }
            else
            {
                throw new AssertionFailedException($"{description} FAILED. Expected {expected}. was {actual}");
            }
        }

        #endregion

        #region Private Data

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}
