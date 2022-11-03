using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceNet6.Exceptions
{
    /// <summary>
    /// Class ClickOnceNotFoundUpdateLocationException.
    /// Implements the <see cref="Exception" />
    /// </summary>
    /// <seealso cref="Exception" />
    public class ClickOnceNotFoundUpdateLocationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClickOnceNotFoundUpdateLocationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ClickOnceNotFoundUpdateLocationException(string message) : base(message)
        {
        }
    }
}
