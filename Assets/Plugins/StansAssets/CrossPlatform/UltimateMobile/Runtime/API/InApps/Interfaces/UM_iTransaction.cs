using System;
using SA.Foundation.Templates;

namespace SA.CrossPlatform.InApp
{
    /// <summary>
    /// Transaction template.
    /// </summary>
    public interface UM_iTransaction
    {
        /// <summary>
        /// A string that uniquely identifies a successful payment transaction.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// A string used to identify a product that can be purchased from within your application.
        /// </summary>
        string ProductId { get; }

        /// <summary>
        /// The date the transaction was added to the payment queue.
        /// </summary>
        /// <value>The timestamp.</value>
        DateTime Timestamp { get; }

        /// <summary>
        /// Object that describes an error which happen during payment transaction.
        /// If transaction was successful property is <c>null</c>
        /// </summary>
        /// <value>The error.</value>
        SA_Error Error { get; }

        /// <summary>
        /// Transaction state.
        /// </summary>
        /// <value>The state.</value>
        UM_TransactionState State { get; }

        /// <summary>
        /// Native transaction template originally provided by the platform specific SDK.
        /// This template was used to fill in all the object properties,
        /// and may contain more platform specific information.
        /// </summary>
        object NativeTemplate { get; }
    }
}
