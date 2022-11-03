using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceNet6.Exceptions
{
    /// <summary>
    /// Class ClickOnceInvalidDeploymentException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class ClickOnceInvalidDeploymentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickOnceDeploymentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickOnceInvalidDeploymentException(string message) : base(message)
        {
        }
    }
}
